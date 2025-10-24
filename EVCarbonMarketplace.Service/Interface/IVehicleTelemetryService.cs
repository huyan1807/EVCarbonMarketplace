using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleTelemetry;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IVehicleTelemetryService
    {
        Task<BaseResponse<IPaginate<VehicleTelemetryResponse>>> GetVehicleTelemetry(int page , int size ,Guid id);
        Task<BaseResponse<bool>> Delete(Guid id);

    }
}
