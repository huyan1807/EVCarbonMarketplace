using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Request.Payment;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Payment;
using EVCarbonMarketplace.Model.Payload.Settings;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Enum;

namespace EVCarbonMarketplace.Service.Implement
{
    public class PaymentService : BaseService<PaymentService>, IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly PayOS _payOS;
        private readonly PayOSSettings _payOSSettings;
        private readonly IConnectionMultiplexer _redis;
        private readonly IEmailSender _emailSender;
        public PaymentService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<PaymentService> logger, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor , 
            IConfiguration config,
            IConnectionMultiplexer redis,
            IOptions<PayOSSettings> payOSSettings,
            IEmailSender emailSender
            ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _config = config;
            _payOSSettings = payOSSettings.Value;
            _payOS = new PayOS(_payOSSettings.ClientId, _payOSSettings.ApiKey, _payOSSettings.ChecksumKey);
            _redis = redis;
            _emailSender = emailSender;
        }

        public async   Task<BaseResponse<string>> Create(PaymentRequest request)
        {
            Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: a => a.Id.Equals(accountId) && a.IsActive == true) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            if (request.Amount < 0.01m)
            {

                throw new BadHttpRequestException("Số tiền phải lớn hơn 0");
            }


            var random = new Random().Next(100, 999); // 3 chữ số
            var baseCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1_000_000;
            var orderCode = long.Parse($"{baseCode}{random}"); // 9 chữ số chắc chắn

            string description = "";
            long expiredAt = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();

            // Tạo signature
            var signatureData = new SortedDictionary<string, object>
                        {
                        { "amount", request.Amount },
                        { "cancelUrl", request.CancelUrl },
                        { "description", description },
                        { "expiredAt", expiredAt },
                        { "orderCode", orderCode },
                        { "returnUrl", request.ReturnUrl }
                            };
            var data = string.Join("&", signatureData.Select(p => $"{p.Key}={p.Value}"));
            var signature = PaymentUtil.ComputeHmacSha256(data, _payOSSettings.ChecksumKey);

            // Tạo PaymentData để gửi đến PayOS
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)request.Amount,
                description: description,
                items: null,
                cancelUrl: request.CancelUrl,
                returnUrl: request.ReturnUrl,
                signature: signature,
                buyerName: account.FullName,
                buyerPhone: account.Phone,
                buyerEmail: account.Email,
                buyerAddress: "VN",
                expiredAt: (int)expiredAt
            );

            var paymentresult = await _payOS.createPaymentLink(paymentData);
            await _redis.GetDatabase().StringSetAsync($"payos:{orderCode}", accountId.ToString(), TimeSpan.FromMinutes(15));
            return new BaseResponse<string>
            {
                Status = StatusCodes.Status201Created.ToString(),
                Message = "Tạo link thanh toán thành công",
                Data = paymentresult.checkoutUrl
            };
        }

        public async Task<BaseResponse<string>> HandleWebhook(WebhookNotification notification)
        {
            try
            {
                if (notification.Success && notification.Data.Code == "00")
                {


                    var orderCode = notification.Data.OrderCode;

                    var amount = notification.Data.Amount;

                    var accountIdStr = await _redis.GetDatabase().StringGetAsync($"payos:{orderCode}");

                    if (accountIdStr.IsNullOrEmpty)
                    {
                        throw new Exception($"Không tìm thấy accountId trong Redis cho orderCode: {orderCode}");
                    }

                    string accountIdString = accountIdStr.ToString();


                    if (!Guid.TryParse(accountIdString, out Guid accountId))
                    {
                        throw new Exception("Dữ liệu Redis không đúng định dạng GUID");
                    }




                    var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                            predicate: a => a.AccountId == accountId && a.IsActive == true,
                            include: w => w.Include(w => w.Account)
                            ) ?? throw new NotFoundException("Không tìm thấy ví");

                    var depositId = Guid.NewGuid();
                    var deposit = new Deposit
                    {
                        Id = depositId,
                        AccountId = accountId,
                        Code = orderCode.ToString(),
                        Description = "Nạp tiền qua PayOS",
                        Amount = amount,
                        IsActive = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                    };
                    await _unitOfWork.GetRepository<Deposit>().InsertAsync(deposit);

                    // Cập nhật số dư ví
                    wallet.Cash += (int)amount;
                    wallet.UpdateAt = TimeUtil.GetCurrentSEATime();
                    _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
                    // Tạo giao dịch (Transaction)
                    var transaction = new Model.Entity.Transaction
                    {
                        Id = Guid.NewGuid(),
                        WalletId = wallet.Id,
                        DepositId = deposit.Id,
                        Type =TransactionEnum.Deposit.ToString(),
                        Description = "Nạp tiền vào ví",
                        Status = "Success",
                        Amount = amount,
                        IsActive = true,
                        CreateAt = TimeUtil.GetCurrentSEATime(),
                    };
                    await _unitOfWork.GetRepository<Model.Entity.Transaction>().InsertAsync(transaction);

                    await _unitOfWork.CommitAsync();

                    var placeholders = new Dictionary<string, string>
                        {
                            { "customerName", wallet.Account.FullName ?? wallet.Account.Email },
                            { "amount", amount.ToString("N0") },
                            { "currency", "VND" },
                            { "transactionId", transaction.Id.ToString() },
                            { "date", TimeUtil.GetCurrentSEATime().ToString("dd/MM/yyyy HH:mm") },
                            { "newBalance", wallet.Cash?.ToString("N0") }
                        };

                    string html = TemplateUtil.GetTemplate("Deposit.html", placeholders);

                    var emailMessage = new EmailMessage()
                    {
                        ToAddress = wallet.Account.Email,
                        Body = html,
                        Subject = $"Xác nhận nạp tiền thành công - Giao dịch #{transaction.Id.ToString().Substring(0, 8)}"
                    };
                    await _emailSender.SendEmailAsync(emailMessage);

                    return new BaseResponse<string>
                    {
                        Status = StatusCodes.Status200OK.ToString(),
                        Message = "Xử lý webhook và nạp tiền thành công"
                    };
                }
                return new BaseResponse<string>
                {
                    Status = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Webhook không hợp lệ hoặc giao dịch không thành công"
                };

            }
            catch (Exception ex)
            {

                return new BaseResponse<string>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = $"Lỗi khi xử lý webhook: {ex.Message}"
                };
            }

        }
    }
}
