using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonEmission;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class CarbonEmissionService : BaseService<CarbonEmissionService>, ICarbonEmissionService
    {
        private readonly IFileReaderService _fileReaderService;
        public CarbonEmissionService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<CarbonEmissionService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IFileReaderService fileReaderService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _fileReaderService = fileReaderService;
        }

        public async Task<BaseResponse<CarbonEmissionResponse>> ApproveEmission(Guid id, CarbonEmissionEnum status)
        {
            var emission = await _unitOfWork.GetRepository<CarbonEmission>()
                .SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true,
                include: x => x.Include(e => e.ElectricVehicle).ThenInclude(ev => ev.Account)
                )
                ?? throw new NotFoundException("Không tìm thấy phát thải");

            var vehicle = await _unitOfWork.GetRepository<ElectricVehicle>()
                .SingleOrDefaultAsync(predicate: x => x.Id == emission.ElectricVehicleId && x.IsActive == true)
                ?? throw new NotFoundException("Không tìm thấy xe điện");

            if (emission.Status != CarbonEmissionEnum.Pending.ToString())
            {
                throw new BadHttpRequestException("Chỉ có thể phê duyệt hoặc từ chối các phát thải đang chờ xử lý");
            }

            if(status.ToString().Equals(CarbonEmissionEnum.Approved.ToString()))
            {
                //
                var credit = Math.Ceiling(((emission.Co2reduced ?? 0) / 1000m) * 100) / 100;
                var carbonCredit = new CarbonCredit
                {
                    Id = Guid.NewGuid(),
                    AccountId = emission.ElectricVehicle.AccountId,
                    CarbonEmissionId = emission.Id,
                    Credits = credit,                
                    IsActive = true,
                    CreateAt = TimeUtil.GetCurrentSEATime(),
                    Status = CarbonCreditEnum.Available.ToString()
                };
                await _unitOfWork.GetRepository<CarbonCredit>().InsertAsync(carbonCredit);
                //

                var wallet = await _unitOfWork.GetRepository<Wallet>()
                    .SingleOrDefaultAsync(predicate: x => x.AccountId == emission.ElectricVehicle.AccountId && x.IsActive == true) 
                    ?? throw new NotFoundException("Không tìm thấy ví cho tài khoản");
                
                wallet.CarbonUnit += credit;
                wallet.UpdateAt = TimeUtil.GetCurrentSEATime();
               _unitOfWork.GetRepository<Wallet>().UpdateAsync(wallet);
                emission.Status = CarbonEmissionEnum.Approved.ToString();
                _unitOfWork.GetRepository<CarbonEmission>().UpdateAsync(emission);
               
            }else
            {
                emission.Status = CarbonEmissionEnum.Rejected.ToString();
                _unitOfWork.GetRepository<CarbonEmission>().UpdateAsync(emission);

            }
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình phê duyệt phát thải ");

            return new BaseResponse<CarbonEmissionResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Phê duyệt phát thải thành công",
                Data = new CarbonEmissionResponse
                {
                    Id = emission.Id,
                    ElectricVehicleId = emission.ElectricVehicleId,
                    DistanceTravelled = emission.DistanceTravelled,
                    EnergyConsumed = emission.EnergyConsumed,
                    Co2reduced = emission.Co2reduced,
                    PeriodStart = emission.PeriodStart,
                    PeriodEnd = emission.PeriodEnd,
                    Status = emission.Status,

                }
            };
        }

        public async Task<BaseResponse<bool>> DeleteEmission(Guid id)
        {
            var emission = await _unitOfWork.GetRepository<CarbonEmission>()
                .SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true
                )
                ?? throw new NotFoundException("Không tìm thấy phát thải");

            emission.IsActive = false;
            emission.DeleteAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<CarbonEmission>().UpdateAsync(emission);

            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình xóa phát thải ");

            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa phát thải thành công",
                Data = true
            };
        }

        public async Task<BaseResponse<IPaginate<CarbonEmissionManageResponse>>> GetAll(int page, int size, CarbonEmissionEnum? status)
        {
            var CarbonEmissions = await _unitOfWork.GetRepository<CarbonEmission>()
                .GetPagingListAsync(
                selector: x => new CarbonEmissionManageResponse
                {
                    Id = x.Id,
                    ElectricVehicleId = x.ElectricVehicleId,
                    DistanceTravelled = x.DistanceTravelled,
                    EnergyConsumed = x.EnergyConsumed,
                    Co2reduced = x.Co2reduced,
                    PeriodStart = x.PeriodStart,
                    PeriodEnd = x.PeriodEnd,
                    Status = x.Status,
                    CreateAt = x.CreateAt,
                    VehicleModel = x.ElectricVehicle.VehicleModel,
                    LicensePlate = x.ElectricVehicle.LicensePlate,
                    AccountId = x.ElectricVehicle.AccountId,
                    OwnerName = x.ElectricVehicle.Account.FullName
                    
                },
                    predicate: x => x.IsActive == true && (status == null || x.Status == status.ToString()),
                    orderBy: x => x.OrderByDescending(c => c.CreateAt),
                    include: c => c.Include(e => e.ElectricVehicle)
                                   .ThenInclude(ev => ev.Account),
                                 
                                        
                    page: page,
                    size: size
                );
            return new BaseResponse<IPaginate<CarbonEmissionManageResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách phát thải thành công",
                Data = CarbonEmissions
            };
        }

        public async Task<BaseResponse<CarbonEmissionDetailResponse>> GetById(Guid id)
        {
            var CEDetail = await _unitOfWork.GetRepository<CarbonEmission>().SingleOrDefaultAsync(

                selector: x => new CarbonEmissionDetailResponse
                {
                    Id = x.Id,
                    Status = x.Status,
                    DistanceTravelled = x.DistanceTravelled,
                    EnergyConsumed = x.EnergyConsumed,
                    Co2Reduced = x.Co2reduced,
                    PeriodStart = x.PeriodStart,
                    PeriodEnd = x.PeriodEnd,
                    CreateAt = x.CreateAt,

                    AccountId = x.ElectricVehicle.Account.Id,
                    OwnerName = x.ElectricVehicle.Account.FullName,
                    OwnerEmail = x.ElectricVehicle.Account.Email,
                    OwnerPhone = x.ElectricVehicle.Account.Phone,

                    ElectricVehicleId = x.ElectricVehicle.Id,
                    VehicleModel = x.ElectricVehicle.VehicleModel,
                    Vin = x.ElectricVehicle.Vin,
                    LicensePlate = x.ElectricVehicle.LicensePlate,
                    Brand = x.ElectricVehicle.Brand,
                    BatteryCapacity = x.ElectricVehicle.BatteryCapacity,
                    VehicleTypeId = x.ElectricVehicle.VehicleTypeId,
                    VehicleTypeName = x.ElectricVehicle.VehicleType.Name,
                    ImageUrl = x.ElectricVehicle.ImageUrl

                },
                predicate: x => x.Id == id && x.IsActive == true,
                include: x=> x.Include(e => e.ElectricVehicle).
                             ThenInclude(ev => ev.Account)
                             

                )?? throw new NotFoundException("Không tìm thấy phát thải");


            return new BaseResponse<CarbonEmissionDetailResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy chi tiết phát thải thành công",
                Data = CEDetail
            };




        }

        public async Task<BaseResponse<CarbonEmissionResponse>> ImportTelemetryFromFileAsync(Guid Id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new BaseResponse<CarbonEmissionResponse>
                {
                    Status = "400",
                    Message = "File không hợp lệ",
                    Data = null
                };
            }

            var EVehicle = await _unitOfWork.GetRepository<ElectricVehicle>()
                .SingleOrDefaultAsync( predicate: x => x.Id == Id && x.IsActive == true)
                ?? throw new NotFoundException("Không tìm thấy xe điện");

            var records = await _fileReaderService.ReadTelemetryFileAsync(file);

            if (records == null || records.Count == 0)
            {
                return new BaseResponse<CarbonEmissionResponse>
                {
                    Status = "400",
                    Message = "Không có dữ liệu trong file",
                    Data = null
                };
            }
            //check xem xe đã có phát thải chưa
            var existingEmission = await _unitOfWork.GetRepository<CarbonEmission>()
                .SingleOrDefaultAsync(predicate: x => x.ElectricVehicleId == Id && x.IsActive == true);
            if (existingEmission != null)
                throw new BadHttpRequestException("Xe đã có phát thải, không thể nhập dữ liệu mới");


            decimal totalDistance = records.Sum(r => r.DistanceTravelled);
            decimal totalEnergy = records.Sum(r => r.EnergyConsumed);

            const decimal EmissionFactorICE = 0.12m;     
            const decimal EmissionFactorElectricity = 0.5m; 

            var co2ICE = totalDistance * EmissionFactorICE;
            var co2EV = totalEnergy * EmissionFactorElectricity;
            var co2Reduced = co2ICE - co2EV;

            var emission = new CarbonEmission
            {
                Id = Guid.NewGuid(),
                ElectricVehicleId = Id,
                DistanceTravelled = totalDistance,
                EnergyConsumed = totalEnergy,
                Co2reduced = co2Reduced,
                PeriodStart = records.Min(r => r.LoggedAt),
                PeriodEnd = records.Max(r => r.LoggedAt),
                CreateAt = TimeUtil.GetCurrentSEATime(),
                IsActive = true,
                Status = CarbonEmissionEnum.Pending.ToString()
            };

            await _unitOfWork.GetRepository<CarbonEmission>().InsertAsync(emission);

            var telemetryRepo = _unitOfWork.GetRepository<VehicleTelemetry>();
            foreach (var record in records)
            {
                var telemetry = new VehicleTelemetry
                {
                    Id = Guid.NewGuid(),
                    ElectricVehicleId = Id,
                    CarbonEmissionId = emission.Id,  
                    LoggedAt = record.LoggedAt,
                    Odometer = int.Parse(record.Odometer.ToString()),
                    DistanceTravelled = record.DistanceTravelled,
                    EnergyConsumed = record.EnergyConsumed,
                    BatteryLevel = record.BatteryLevel,
                    CreateAt = TimeUtil.GetCurrentSEATime(),
                    IsActive = true
                };

                await telemetryRepo.InsertAsync(telemetry);
            }

            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình tạo phát thải ");

            return new BaseResponse<CarbonEmissionResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo phát thải thành công",
                Data = new CarbonEmissionResponse
                {
                    Id = emission.Id,
                    ElectricVehicleId = emission.ElectricVehicleId,
                    DistanceTravelled = emission.DistanceTravelled,
                    EnergyConsumed = emission.EnergyConsumed,
                    Co2reduced = emission.Co2reduced,
                    PeriodStart = emission.PeriodStart,
                    PeriodEnd = emission.PeriodEnd,
                    Status = emission.Status,

                }
            };
        }



    }
}
