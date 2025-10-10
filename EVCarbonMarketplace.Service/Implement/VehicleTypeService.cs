using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleType;
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
    public class VehicleTypeService : BaseService<VehicleTypeService>, IVehicleTypeService
    {
        public VehicleTypeService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<VehicleTypeService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<VehicleTypeResponse>> Create(VehicleTypeRequest request)
        {
            var existingVehicleType = await _unitOfWork.GetRepository<VehicleType>().SingleOrDefaultAsync(predicate: x => x.Name.Equals(request.Name) && x.IsActive == true);
            if (existingVehicleType != null)
            {
                return new BaseResponse<VehicleTypeResponse>
                {
                    Status = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Loại xe đã tồn tại",
                    Data = null
                };
            }
            var vehicleType = new VehicleType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,          
                IsActive = true,
                CreateAt =TimeUtil.GetCurrentSEATime()
            };

            await _unitOfWork.GetRepository<VehicleType>().InsertAsync(vehicleType);
            var iSucces = await _unitOfWork.CommitAsync() > 0;
            if (!iSucces)
            {
                return new BaseResponse<VehicleTypeResponse>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Tạo loại xe thất bại",
                    Data = null
                };
            }

            var response = new VehicleTypeResponse
            {
                Id = vehicleType.Id,
                Name = vehicleType.Name,
                IsActive = vehicleType.IsActive,
                CreateAt = vehicleType.CreateAt
            };

            return new BaseResponse<VehicleTypeResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo loại xe thành công",
                Data = response
            };
        }


        public async Task<BaseResponse<bool>> Delete(Guid id)
        {
            var vehicleType = await _unitOfWork.GetRepository<VehicleType>().SingleOrDefaultAsync( predicate:x => x.Id == id && x.IsActive == true);
            if (vehicleType == null)
            {
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status404NotFound.ToString(),
                    Message = "Không tìm thấy loại xe",
                    Data = false
                };
            }

            vehicleType.IsActive = false;
            vehicleType.DeleteAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<VehicleType>().UpdateAsync(vehicleType);
            var iSucces = await _unitOfWork.CommitAsync() > 0;
            if (!iSucces)
            {
                return new BaseResponse<bool>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Xóa loại xe thất bại",
                    Data = false
                };
            }

            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa loại xe thành công",
                Data = true
            };

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
