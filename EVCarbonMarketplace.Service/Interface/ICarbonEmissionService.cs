using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonEmission;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface ICarbonEmissionService
    {
        Task<BaseResponse<CarbonEmissionResponse>> ImportTelemetryFromFileAsync(Guid Id, IFormFile file);

        Task<BaseResponse<IPaginate<CarbonEmissionManageResponse>>> GetAll(int page,int size , CarbonEmissionEnum? status);
        Task<BaseResponse<CarbonEmissionDetailResponse>> GetById(Guid id);

        Task<BaseResponse<CarbonEmissionResponse>> ApproveEmission(Guid id ,CarbonEmissionEnum status);

    }
}
