using EVCarbonMarketplace.Model.Payload.Request.BankAccount;
using EVCarbonMarketplace.Model.Payload.Response.BankAccount;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Paginate;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IBankAccountService
    {
        Task<BaseResponse<BankAccountResponse>> CreateBankAccount(CreateBankAccountRequest request);
        Task<BaseResponse<IPaginate<BankAccountResponse>>> GetBankAccounts(int page , int size);
        Task<BaseResponse<BankAccountResponse>> SetDefaultBankAccount(Guid bankAccountId);
        Task<BaseResponse<BankAccountResponse>> GetDefaultBankAccount();
        Task<BaseResponse<bool>> DeleteBankAccount(Guid bankAccountId);

    }
}
