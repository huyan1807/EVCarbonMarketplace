using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.BankAccount;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.BankAccount;
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
    public class BankAccountService : BaseService<BankAccountService>, IBankAccountService
    {
        public BankAccountService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<BankAccountService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<BankAccountResponse>> CreateBankAccount(CreateBankAccountRequest request)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản");

            var existingBankAccount = await _unitOfWork.GetRepository<BankAccount>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId && x.BankAccountNumber.Equals(request.BankAccountNumber)  && x.IsActive == true);
            if (existingBankAccount != null)
            {
               return new BaseResponse<BankAccountResponse>
               {
                   Status = StatusCodes.Status400BadRequest.ToString(),
                   Message = "Tài khoản ngân hàng đã tồn tại",
                   Data = null
               };
            }

            var bankAccount = _mapper.Map<BankAccount>(request);
            bankAccount.AccountId = accountId.Value;
            await _unitOfWork.GetRepository<BankAccount>().InsertAsync(bankAccount);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if(!iSuccess) throw new Exception("có lỗi trong quá trình thêm tài khoản ngân hàng");

            var response = _mapper.Map<BankAccountResponse>(bankAccount);
            return new BaseResponse<BankAccountResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Thêm tài khoản ngân hàng thành công",
                Data = response
            };

        }

        public async Task<BaseResponse<bool>> DeleteBankAccount(Guid bankAccountId)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản");

            var bankAccount = await _unitOfWork.GetRepository<BankAccount>().SingleOrDefaultAsync(predicate: x => x.Id == bankAccountId && x.AccountId == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản ngân hàng");

            if (bankAccount.IsDefault == true)
            {
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Không thể xóa tài khoản ngân hàng mặc định, vui lòng đặt một tài khoản khác làm mặc định trước khi xóa",
                    Data = false
                };
            }

            bankAccount.IsActive = false;
            _unitOfWork.GetRepository<BankAccount>().UpdateAsync(bankAccount);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("có lỗi trong quá trình xóa tài khoản ngân hàng");

            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa tài khoản ngân hàng thành công",
                Data = true
            };

        }

        public async Task<BaseResponse<IPaginate<BankAccountResponse>>> GetBankAccounts(int page, int size)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản");
            var bankAccounts = await _unitOfWork.GetRepository<BankAccount>().GetPagingListAsync(

                selector: b => new BankAccountResponse
                {
                    Id = b.Id,
                    BankName = b.BankName,
                    BankAccountNumber = b.BankAccountNumber,
                    BankAccountHolder = b.BankAccountHolder,
                    IsDefault = b.IsDefault.Value,
                    CreateAt = b.CreateAt.Value,
                    BankCode = b.BankCode,
                    LogoUrl = b.LogoUrl


                },
                predicate: x => x.AccountId == accountId && x.IsActive == true,
                page: page,
                size: size
                );
            if (bankAccounts.Items.Count == 0)
                return new BaseResponse<IPaginate<BankAccountResponse>>
                {
                    Status = StatusCodes.Status404NotFound.ToString(),
                    Message = "không tìm thấy tài khoản ngân hàng",
                    Data = null
                };
            return new BaseResponse<IPaginate<BankAccountResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tài khoản ngân hàng thành công",
                Data = bankAccounts
            };




        }

        public async Task<BaseResponse<BankAccountResponse>> GetDefaultBankAccount()
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản");

            var defaultBankAccount = await _unitOfWork.GetRepository<BankAccount>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId && x.IsDefault == true && x.IsActive == true);
            if (defaultBankAccount == null)
            {
                return new BaseResponse<BankAccountResponse>
                {
                    Status = StatusCodes.Status404NotFound.ToString(),
                    Message = "Chưa có tài khoản ngân hàng mặc định, vui lòng chọn một tài khoản để đặt mặc định",
                    Data = null
                };
            }

            var response = _mapper.Map<BankAccountResponse>(defaultBankAccount);
            return new BaseResponse<BankAccountResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy tài khoản ngân hàng mặc định thành công",
                Data = response
            };

        }

        public async Task<BaseResponse<BankAccountResponse>> SetDefaultBankAccount(Guid bankAccountId)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản");

            var bankAccount = await _unitOfWork.GetRepository<BankAccount>().SingleOrDefaultAsync(predicate: x => x.Id == bankAccountId && x.AccountId == accountId && x.IsActive == true)
                ?? throw new NotFoundException("không tìm thấy tài khoản ngân hàng");

            var currentDefaultBankAccount = await _unitOfWork.GetRepository<BankAccount>().SingleOrDefaultAsync(predicate: x => x.AccountId == accountId && x.IsDefault == true && x.IsActive == true);
            if (currentDefaultBankAccount != null)
            {
                currentDefaultBankAccount.IsDefault = false;
                _unitOfWork.GetRepository<BankAccount>().UpdateAsync(currentDefaultBankAccount);
            }

            bankAccount.IsDefault = true;
            _unitOfWork.GetRepository<BankAccount>().UpdateAsync(bankAccount);

            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("có lỗi trong quá trình đặt tài khoản ngân hàng mặc định");

            var response = _mapper.Map<BankAccountResponse>(bankAccount);
            return new BaseResponse<BankAccountResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đặt tài khoản ngân hàng mặc định thành công",
                Data = response
            };


        }
    }
}
