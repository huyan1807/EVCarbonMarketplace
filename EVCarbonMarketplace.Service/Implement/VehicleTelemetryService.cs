using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleTelemetry;
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
    public class VehicleTelemetryService : BaseService<VehicleTelemetryService>, IVehicleTelemetryService
    {
        public VehicleTelemetryService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<VehicleTelemetryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<bool>> Delete(Guid id)
        {

            var telemetries = await _unitOfWork.GetRepository<VehicleTelemetry>().GetListAsync(
                predicate: x => x.ElectricVehicleId == id && x.IsActive == true
            );

            if (telemetries == null || !telemetries.Any())
            {
                return new BaseResponse<bool>
                {
                    Message = "Không có dữ liệu hành trình nào của xe cần xóa",
                    Status = StatusCodes.Status404NotFound.ToString(),
                    Data = false
                };
            }

            foreach (var telemetry in telemetries)
            {
                telemetry.IsActive = false;
                telemetry.DeleteAt = TimeUtil.GetCurrentSEATime();
                _unitOfWork.GetRepository<VehicleTelemetry>().UpdateAsync(telemetry);
            }

            var result = await _unitOfWork.CommitAsync() > 0;

            if (!result)
            {
                return new BaseResponse<bool>
                {
                    Message = "Xoá dữ liệu di chuyển của xe thất bại",
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Data = false
                };
            }

            return new BaseResponse<bool>
            {
                Message = "Đã xoá toàn bộ dữ liệu di chuyển của xe thành công",
                Status = StatusCodes.Status200OK.ToString(),
                Data = true
            };

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
                    Odometer = x.Odometer,
                    IsActive = x.IsActive.ToString()
                },
                predicate: x => x.ElectricVehicleId.Equals(id),
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
