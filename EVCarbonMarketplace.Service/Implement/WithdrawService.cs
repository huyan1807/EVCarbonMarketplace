using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.Withdraw;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Withdraw;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class WithdrawService : BaseService<WithdrawService>, IWithdrawService
    {
        private readonly IUploadService _uploadService;
        public WithdrawService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<WithdrawService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadService uploadService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
        }

        public async Task<BaseResponse<IPaginate<WithdrawResponse>>> GetAllWithdraw(int page, int size, WithdrawEnum? status)
        {
          
            var withdraws = await _unitOfWork.GetRepository<Withdraw>().GetPagingListAsync(
                selector: x => _mapper.Map<WithdrawResponse>(x),

                predicate: x =>  (status == null || x.Status == status.ToString()),
                include: source => source.Include(w => w.BankAccount),
                orderBy: source => source.OrderByDescending(w => w.CreateAt),
                page: page,
                size: size
                );
            return new BaseResponse<IPaginate<WithdrawResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy lịch sử rút tiền thành công",
                Data = withdraws
            };

        }

        public async Task<BaseResponse<IPaginate<WithdrawResponse>>> GetWithdrawHistory(int page, int size, WithdrawEnum? status)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var withdraws = await _unitOfWork.GetRepository<Withdraw>().GetPagingListAsync(
                selector: x => _mapper.Map<WithdrawResponse>(x),

                predicate: x => x.AccountId == accountId && (status == null || x.Status == status.ToString()),
                include: source => source.Include(w => w.BankAccount),
                orderBy: source => source.OrderByDescending(w => w.CreateAt),
                page: page,
                size: size
                );
            return new BaseResponse<IPaginate<WithdrawResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy lịch sử rút tiền thành công",
                Data = withdraws
            };
        }

        public async Task<BaseResponse<WithdrawResponse>> RequestWithdraw(WithdrawRequest request)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var bankAccount = await _unitOfWork.GetRepository<BankAccount>().SingleOrDefaultAsync(
                predicate: x => x.Id == request.BankAccountId && x.AccountId == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản ngân hàng");
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy ví");

            if (request.Amount <= 0)
                throw new BadHttpRequestException("Số tiền rút phải lớn hơn 0");
            if (wallet.Cash < request.Amount)
                throw new BadHttpRequestException("Số dư tài khoản không đủ");
            var withdraw = new Withdraw
            {
                Id = Guid.NewGuid(),
                AccountId = accountId.Value,
                BankAccountId = request.BankAccountId,
                Amount = request.Amount,
                Status = WithdrawEnum.Pending.ToString(),
                CreateAt = TimeUtil.GetCurrentSEATime(),

            };
            await _unitOfWork.GetRepository<Withdraw>().InsertAsync(withdraw);
            wallet.Cash -= request.Amount;
            _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                Type = TransactionEnum.Withdraw.ToString(),
                Status = TransactionStatusEnum.Pending.ToString(),
                Amount = request.Amount,
                IsActive = true,
                CreateAt = TimeUtil.GetCurrentSEATime(),
                Description = $"Rút {request.Amount:N0} VNĐ về tài khoản ngân hàng"
            };
            await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("Có lỗi trong quá trình rút tiền");
            var withdrawWithBank = await _unitOfWork.GetRepository<Withdraw>().SingleOrDefaultAsync(
                    predicate: x => x.Id == withdraw.Id,
                    include: x => x.Include(w => w.BankAccount)
                );
            var response = _mapper.Map<WithdrawResponse>(withdrawWithBank);

            return new BaseResponse<WithdrawResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Yêu cầu rút tiền thành công",
                Data = response
            };


        }

        public async Task<BaseResponse<WithdrawResponse>> UpdateWithdrawStatus(UpdateWithdrawRequest request)
        {
            var withdraw = await _unitOfWork.GetRepository<Withdraw>().SingleOrDefaultAsync(
                predicate: x => x.Id == request.Id,
                include: source => source.Include(w => w.Account).Include(w => w.BankAccount)
                ) ?? throw new NotFoundException("Không tìm thấy yêu cầu rút tiền");
            var wallet = await _unitOfWork.GetRepository<Wallet>()
               .SingleOrDefaultAsync(predicate: w => w.AccountId == withdraw.AccountId && w.IsActive == true)
               ?? throw new NotFoundException("Không tìm thấy ví");
            var transaction = await _unitOfWork.GetRepository<Transaction>()
                 .SingleOrDefaultAsync(predicate: t => t.WalletId == wallet.Id
                                  && t.Amount == withdraw.Amount
                                  && t.Type == TransactionEnum.Withdraw.ToString()
                                  && t.Status == TransactionStatusEnum.Pending.ToString())
                ?? throw new NotFoundException("Không tìm thấy transaction của yêu cầu rút tiền");
            if (withdraw.Status != WithdrawEnum.Pending.ToString())
                throw new BadHttpRequestException("Yêu cầu rút tiền đã được xử lý");

            if (request.Status == WithdrawEnum.Approved)
            {
                if (request.ProofUrl == null) throw new BadHttpRequestException("Vui lòng cung cấp hình ảnh chứng từ");

                withdraw.Status = WithdrawEnum.Approved.ToString();
                withdraw.ProofUrl = await _uploadService.UploadImage(request.ProofUrl);
                withdraw.UpdateAt = TimeUtil.GetCurrentSEATime();

                transaction.Status = TransactionStatusEnum.Success.ToString();
                transaction.UpdateAt = TimeUtil.GetCurrentSEATime();
            }
            else if (request.Status == WithdrawEnum.Rejected)
            {
                withdraw.Status = WithdrawEnum.Rejected.ToString();
                withdraw.Description = request.Description;
                withdraw.UpdateAt = TimeUtil.GetCurrentSEATime();

                transaction.Status = TransactionStatusEnum.Fail.ToString();
                transaction.UpdateAt = TimeUtil.GetCurrentSEATime();

                wallet.Cash += withdraw.Amount.Value;
                _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
            }
            else
            {
                throw new BadHttpRequestException("Trạng thái không hợp lệ");
            }
            _unitOfWork.GetRepository<Withdraw>().UpdateAsync(withdraw);
            _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("Có lỗi trong quá trình xử lý yêu cầu rút tiền");
            var response = _mapper.Map<WithdrawResponse>(withdraw);


            return new BaseResponse<WithdrawResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Cập nhật trạng thái rút tiền thành công",
                Data = response
            };



        }
    }
}
