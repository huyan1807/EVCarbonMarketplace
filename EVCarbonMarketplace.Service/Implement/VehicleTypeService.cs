using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleType;
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
    public class VehicleTypeService : BaseService<VehicleTypeService>, IVehicleTypeService
    {
        public VehicleTypeService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<VehicleTypeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }
        public async Task<BaseResponse<IPaginate<VehicleTypeResponse>>> GetAll(int page, int size)
        {
            var vehicleTypes = await _unitOfWork.GetRepository<VehicleType>().GetPagingListAsync(
                selector: x => new VehicleTypeResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    CreateAt = x.CreateAt
                },
                predicate: x => x.IsActive == true,
                orderBy: x => x.OrderByDescending(x => x.CreateAt),
                page: page,
                size: size
            );
            return new BaseResponse<IPaginate<VehicleTypeResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách loại xe thành công",
                Data = vehicleTypes
            };

        }
      
    }
}
