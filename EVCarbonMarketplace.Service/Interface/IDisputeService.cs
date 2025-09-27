using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.Dispute;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Dispute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IDisputeService
    {
        List<KeyValuePair<string, string>> GetDisputeTypes();
        Task<BaseResponse<DisputeResponse>> Create(DisputeRequest request);
        Task<BaseResponse<DisputeResponse>> Update(UpdateDisputeStatusRequest request);
        Task<BaseResponse<IPaginate<DisputeResponse>>> GetMyDisputes(int page , int size,DisputeTypeEnum? type, DisputeStatusEnum? status);

        Task<BaseResponse<DisputeDetailResponse>> GetById(Guid id);

        Task<BaseResponse<IPaginate<DisputeResponse>>> GetAll(int page, int size,DisputeTypeEnum? type ,DisputeStatusEnum? status);
        Task<BaseResponse<bool>> Delete(Guid id);


    }
}
