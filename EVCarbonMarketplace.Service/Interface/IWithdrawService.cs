using EVCarbonMarketplace.Model.Payload.Request.Withdraw;
using EVCarbonMarketplace.Model.Payload.Response.Withdraw;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Enum;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IWithdrawService
    {
        Task<BaseResponse<WithdrawResponse>> RequestWithdraw(WithdrawRequest request);
        Task<BaseResponse<IPaginate<WithdrawResponse>>> GetWithdrawHistory(int page, int size , WithdrawEnum? status);
        Task<BaseResponse<WithdrawResponse>> UpdateWithdrawStatus(UpdateWithdrawRequest request );

        Task<BaseResponse<IPaginate<WithdrawResponse>>> GetAllWithdraw(int page, int size, WithdrawEnum? status);

    }
}
