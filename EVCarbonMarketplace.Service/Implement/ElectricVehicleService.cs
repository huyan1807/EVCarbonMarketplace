using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response.User;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class ElectricVehicleService : BaseService<ElectricVehicleService>, IElectricVehicleService
    {
        private readonly IUploadService _uploadService;
        public ElectricVehicleService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<ElectricVehicleService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor , IUploadService uploadService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
        }

        public async Task<BaseResponse<ElectricVehicleResponse>> ChangeImage(Guid? id, IFormFile file)
        {
            Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                          predicate: a => a.Id == accountId && a.IsActive == true) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            var EVhicle = await _unitOfWork.GetRepository<ElectricVehicle>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true && x.AccountId == accountId,
                include: x => x.Include(x => x.VehicleType)
                ) ?? throw new NotFoundException("Không tìm thấy xe điện");

            EVhicle.ImageUrl = await _uploadService.UploadImage(file);
            EVhicle.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<ElectricVehicle>().UpdateAsync(EVhicle);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đổi ảnh xe");
            var evResponse = _mapper.Map<ElectricVehicleResponse>(EVhicle);
            evResponse.VehicleType = EVhicle.VehicleType.Name;
            return new BaseResponse<ElectricVehicleResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đổi ảnh xe điện thành công",
                Data = evResponse
            };

        }

        public async Task<BaseResponse<ElectricVehicleResponse>> Create(ElectricVehicleRequest request)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            var vehicleType = await _unitOfWork.GetRepository<VehicleType>().SingleOrDefaultAsync(
               predicate: x => x.Id == request.VehicleTypeId && x.IsActive == true
               ) ?? throw new NotFoundException("Không tìm thấy loại xe");
            var existingEV = await _unitOfWork.GetRepository<ElectricVehicle>().SingleOrDefaultAsync(
                predicate: x => x.Vin.Equals(request.Vin) && x.IsActive == true
                );
            if (existingEV != null) throw new BadHttpRequestException("Xe điện này đã tồn tại trong hệ thống");

            if (request.ImageUrl == null) throw new BadHttpRequestException("Ảnh xe không được để trống");
            var ev = _mapper.Map<ElectricVehicle>(request);
            ev.ImageUrl = await _uploadService.UploadImage(request.ImageUrl);
            ev.AccountId = accountId;

            await _unitOfWork.GetRepository<ElectricVehicle>().InsertAsync(ev);

            var IsSuccess = await _unitOfWork.CommitAsync() > 0;
            if(!IsSuccess) throw new BadHttpRequestException("Có lỗi trong quá trình tạo xe điện");

            var evResponse = _mapper.Map<ElectricVehicleResponse>(ev);

           
            evResponse.VehicleType = vehicleType.Name;

            return new BaseResponse<ElectricVehicleResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo xe điện thành công",
                Data = evResponse
            };

        }

        public async Task<BaseResponse<bool>> Delete(Guid id)
        {
            var EVehicle = await _unitOfWork.GetRepository<ElectricVehicle>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy xe điện");
            var credit = await _unitOfWork.GetRepository<CarbonEmission>().SingleOrDefaultAsync(
                predicate: x => x.ElectricVehicleId == id && x.IsActive == true && x.Status.Equals(CarbonEmissionEnum.Approved.ToString())
                );
            if (credit != null) throw new NotFoundException("Không thể xóa xe điện đã phát sinh tín chỉ carbon");
            EVehicle.IsActive = false;
            EVehicle.DeleteAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<ElectricVehicle>().UpdateAsync(EVehicle);
            var IsSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!IsSuccess) throw new BadHttpRequestException("Có lỗi trong quá trình xóa xe điện");
            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa xe điện thành công",
                Data = true
            };

        }

        public async Task<BaseResponse<IPaginate<ElectricVehicleResponse>>> GetAll(int page, int size)
        {

            if (page < 1 || size < 1)
            {
                throw new BadHttpRequestException("Số trang và số lượng trong trang phải lớn hơn hoặc bằng 1");
            }

            var EVehicle = await _unitOfWork.GetRepository<ElectricVehicle>().GetPagingListAsync(
                selector: e => new ElectricVehicleResponse
                {
                    Id = e.Id,
                    BatteryCapacity = e.BatteryCapacity,
                    ImageUrl = e.ImageUrl,
                    Brand = e.Brand,
                    LicensePlate = e.LicensePlate,
                    Odometer = e.Odometer,
                    VehicleModel = e.VehicleModel,
                    VehicleType = e.VehicleType.Name,
                    VehicleTypeId = e.VehicleTypeId,
                    Vin = e.Vin,                 
                },
                predicate: x => x.IsActive == true,
                include: x => x.Include(x => x.VehicleType).Include(x => x.Account),
                orderBy: x => x.OrderByDescending(x => x.CreateAt),
                page: page,
                size: size
                );

            return new BaseResponse<IPaginate<ElectricVehicleResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách xe điện thành công",
                Data = EVehicle
            };



        }

        public async Task<BaseResponse<ElectricVehicleResponse>> GetById(Guid id)
        {
           
            var EVehicle = await _unitOfWork.GetRepository<ElectricVehicle>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true,
                include: x => x.Include(x => x.VehicleType)
                ) ?? throw new NotFoundException("Không tìm thấy xe điện");
            var evResponse = _mapper.Map<ElectricVehicleResponse>(EVehicle);
            evResponse.VehicleType = EVehicle.VehicleType.Name;
            return new BaseResponse<ElectricVehicleResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy thông tin xe điện thành công",
                Data = evResponse
            };

        }

        public async Task<BaseResponse<IPaginate<ElectricVehicleResponse>>> GetMyEVehicles(int page, int size)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            if (page < 1 || size < 1)
            {
                throw new BadHttpRequestException("Số trang và số lượng trong trang phải lớn hơn hoặc bằng 1");
            }

            var EVehicle = await _unitOfWork.GetRepository<ElectricVehicle>().GetPagingListAsync(
                selector: e => new ElectricVehicleResponse
                {
                    Id = e.Id,
                    BatteryCapacity = e.BatteryCapacity,
                    ImageUrl = e.ImageUrl,
                    Brand = e.Brand,
                    LicensePlate = e.LicensePlate,
                    Odometer = e.Odometer,
                    VehicleModel = e.VehicleModel,
                    VehicleType = e.VehicleType.Name,
                    VehicleTypeId = e.VehicleTypeId,
                    Vin = e.Vin,
                },
                predicate: x => x.IsActive == true && x.AccountId == accountId,
                include: x => x.Include(x => x.VehicleType).Include(x => x.Account),
                orderBy: x => x.OrderByDescending(x => x.CreateAt),
                page: page,
                size: size
                );
            return new BaseResponse<IPaginate<ElectricVehicleResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách xe điện của tôi thành công",
                Data = EVehicle
            };


        }

        public async Task<BaseResponse<ElectricVehicleResponse>> Update(Guid id, ElectricVehicleUpdateRequest request)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
                
            var vehicleType = await _unitOfWork.GetRepository<VehicleType>().SingleOrDefaultAsync(
               predicate: x => x.Id == request.VehicleTypeId && x.IsActive == true
               ) ?? throw new NotFoundException("Không tìm thấy loại xe");

            var EVehicle = await _unitOfWork.GetRepository<ElectricVehicle>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true && x.AccountId == accountId
                ) ?? throw new NotFoundException("Không tìm thấy xe điện");

             var ev = _mapper.Map(request, EVehicle);

             _unitOfWork.GetRepository<ElectricVehicle>().UpdateAsync(ev);

            var IsSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!IsSuccess) throw new BadHttpRequestException("Có lỗi trong quá trình cập nhật xe điện");
            var evResponse = _mapper.Map<ElectricVehicleResponse>(ev);
            evResponse.VehicleType = vehicleType.Name;
            return new BaseResponse<ElectricVehicleResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Cập nhật xe điện thành công",
                Data = evResponse
            };

        }
    }
}
