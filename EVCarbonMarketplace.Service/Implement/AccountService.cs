using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Settings;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class AccountService : BaseService<AccountService>, IAccountService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IEmailSender _emailSender;
        private readonly IUploadService _uploadService;

        public AccountService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork,
            ILogger<AccountService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConnectionMultiplexer redis,
            IEmailSender emailSender,
            IUploadService uploadService
            ) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _redis = redis;
            _emailSender = emailSender;
            _uploadService = uploadService;
        }

        public async Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();
            if (accounts.Any(a => a.Username.Equals(request.UserName)))
                throw new BadHttpRequestException("Tên đăng nhập đã tồn tại");
            if (accounts.Any(a => a.Email.Equals(request.Email)))
                throw new BadHttpRequestException("Email đã tồn tại");
            if (accounts.Any(a => a.Phone.Equals(request.Phone)))
                throw new BadHttpRequestException("Số điện thoại đã tồn tại");
            var redisDb = _redis.GetDatabase();
            if (redisDb == null) throw new RedisServerException("Không thể kết nối tới Redis");

            var key = "emailOtp" + request.Email;
            var stored = await redisDb.StringGetAsync(key);
            if (string.IsNullOrEmpty(stored))
                throw new NotFoundException("Không tìm thấy mã OTP");
            if (!string.IsNullOrEmpty(stored))
                throw new BadHttpRequestException("Mã OTP không chính xác");

            var account = _mapper.Map<Account>(request);
            account.AvatarUrl = await _uploadService.UploadImage(request.AvatarUrl);

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            // thiếu tạo ví 


            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            await redisDb.KeyDeleteAsync(key);

            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đăng kí tài khoản");

            return new BaseResponse<RegisterResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo tài khoản thành công",
                Data = _mapper.Map<RegisterResponse>(account)
            };



        }

        public async Task<BaseResponse<bool>> SendOtp(string email)
        {
            if (!EmailUtil.IsValidEmail(email))
            {
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Email không đúng định dạng",
                    Data = false
                };
            }
            var redisDb = _redis.GetDatabase();
            var key = "emailOtp:" + email;
            var existingOTP = await redisDb.StringGetAsync(key);
            if (!string.IsNullOrEmpty(existingOTP))
            {
                return new BaseResponse<bool>()
                {
                    Status = StatusCodes.Status409Conflict.ToString(),
                    Message = "Mã OTP đã được gửi, vui lòng chờ một lát",
                    Data = false
                };
            }
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();
            var otp = OtpUtil.GenerateOtp();

            var placeholders = new Dictionary<string, string>
            {
                { "otp", otp },
                { "email", email }
            };

            var html = EmailUtil.GetTemplate("Email.html", placeholders);

            var emailMessage = new EmailMessage()
            {
                ToAddress = email,
                Body = html,
                Subject = otp + " là mã xác thực của bạn"
            };
            await _emailSender.SendEmailAsync(emailMessage);
            var redisSuccess = await redisDb.StringSetAsync(key, otp, TimeSpan.FromMinutes(5));
            if (!redisSuccess) throw new BadHttpRequestException("Không thể lưu mã OTP");

            return new BaseResponse<bool>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Gửi mã OTP thành công",
                Data = true
            };
        }
    }
}
