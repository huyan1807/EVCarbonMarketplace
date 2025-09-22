using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleTelemetry;
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
    public class VehicleTelemetryService : BaseService<VehicleTelemetryService>, IVehicleTelemetryService
    {
        public VehicleTelemetryService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<VehicleTelemetryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<IPaginate<VehicleTelemetryResponse>>> GetVehicleTelemetry(int page,int size,Guid id)
        {
            var telemetries = await _unitOfWork.GetRepository<VehicleTelemetry>().GetPagingListAsync(
                selector: x => new VehicleTelemetryResponse
                {
                    Id = x.Id,
                    BatteryLevel = x.BatteryLevel,
                    DistanceTravelled = x.DistanceTravelled,
                    EnergyConsumed = x.EnergyConsumed,
                    LoggedAt = x.LoggedAt,
                    Odometer = x.Odometer
                },
                predicate: x => x.IsActive == true && x.ElectricVehicleId.Equals(id),
                page: page,
                size: size
                );
            return new BaseResponse<IPaginate<VehicleTelemetryResponse>>()
            {
                Message = "Lấy dữ liệu di chuyển của xe thành công",
                Status = StatusCodes.Status200OK.ToString(),
                Data = telemetries

            };
        }
    }
}
