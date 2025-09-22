using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonCredit;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class CarbonCreditService : BaseService<CarbonCreditService>, ICarbonCreditService
    {
        public CarbonCreditService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<CarbonCreditService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<BaseResponse<IPaginate<CarbonCreditManageResponse>>> GetAllCredits(int page, int size , CarbonCreditEnum? status)
        {
            var credits = await _unitOfWork.GetRepository<CarbonCredit>().GetPagingListAsync(

                selector: x => new CarbonCreditManageResponse
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    OwnerName = x.Account.FullName,
                    Brand = x.CarbonEmission.ElectricVehicle.Brand,
                    CarbonEmissionId = x.CarbonEmissionId,
                    CreateAt = x.CreateAt,
                    Credits = x.Credits,
                    Status = x.Status.ToString(),
                    ElectricVehicleId = x.CarbonEmission.ElectricVehicleId,
                    ImageUrl = x.CarbonEmission.ElectricVehicle.ImageUrl,
                    LicensePlate = x.CarbonEmission.ElectricVehicle.LicensePlate,
                    VehicleModel = x.CarbonEmission.ElectricVehicle.VehicleModel,
                    OwnerEmail = x.Account.Email,
                    OwnerPhone = x.Account.Phone,
                    VehicleType = x.CarbonEmission.ElectricVehicle.VehicleType.Name


                },
                predicate: x => x.IsActive == true && (status == null || x.Status.Equals(status.ToString())),
                include: x => x.Include(x => x.Account).Include(c => c.CarbonEmission).ThenInclude(e => e.ElectricVehicle),
                page: page,
                size: size

                );
            return new BaseResponse<IPaginate<CarbonCreditManageResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tín chỉ carbon thành công",
                Data = credits
            };

        }

        public async Task<BaseResponse<CarbonCreditResponse>> GetCreditDetail(Guid id)
        {
            var credit = await _unitOfWork.GetRepository<CarbonCredit>().SingleOrDefaultAsync(
                        selector: s => new CarbonCreditResponse
                        {
                            Id = s.Id,
                            CarbonEmissionId = s.CarbonEmissionId,
                            Credits = s.Credits,
                            Status = s.Status.ToString(),
                            CreateAt = s.CreateAt,
                            BatteryCapacity = s.CarbonEmission.ElectricVehicle.BatteryCapacity,
                            Brand = s.CarbonEmission.ElectricVehicle.Brand,
                            VehicleModel = s.CarbonEmission.ElectricVehicle.VehicleModel,
                            LicensePlate = s.CarbonEmission.ElectricVehicle.LicensePlate,
                            ElectricVehicleId = s.CarbonEmission.ElectricVehicleId,
                            PeriodStart = s.CarbonEmission.PeriodStart,
                            PeriodEnd = s.CarbonEmission.PeriodEnd,
                            Co2Reduced = s.CarbonEmission.Co2reduced
                        },
                        predicate: x => x.Id == id && x.IsActive == true,   // chỉ lấy theo Id
                        include: x => x.Include(c => c.CarbonEmission)
                                       .ThenInclude(e => e.ElectricVehicle)
                    );
            if (credit == null) throw new NotFoundException("Không tìm thấy tín chỉ carbon");

            return new BaseResponse<CarbonCreditResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy chi tiết tín chỉ carbon thành công",
                Data = credit 
            };
        }

        public async Task<BaseResponse<IPaginate<CarbonCreditResponse>>> GetMyCredits(CarbonCreditEnum? status)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var credits = await _unitOfWork.GetRepository<CarbonCredit>().GetPagingListAsync
                (

                selector: s => new CarbonCreditResponse
                {
                    Id = s.Id,
                    CarbonEmissionId = s.CarbonEmissionId,
                    Credits = s.Credits,
                    Status = s.Status.ToString(),
                    CreateAt = s.CreateAt,
                    BatteryCapacity = s.CarbonEmission.ElectricVehicle.BatteryCapacity,
                    Brand = s.CarbonEmission.ElectricVehicle.Brand,
                    VehicleModel = s.CarbonEmission.ElectricVehicle.VehicleModel,
                    LicensePlate = s.CarbonEmission.ElectricVehicle.LicensePlate,
                    ElectricVehicleId = s.CarbonEmission.ElectricVehicleId,
                    PeriodStart = s.CarbonEmission.PeriodStart,
                    PeriodEnd = s.CarbonEmission.PeriodEnd,
                    Co2Reduced = s.CarbonEmission.Co2reduced


                },

                predicate: x => x.AccountId == accountId && x.IsActive == true && (status ==null || x.Status.Equals(status.ToString())),

                include: x => x.Include(c => c.CarbonEmission).ThenInclude(e => e.ElectricVehicle)                            
                );

            return new BaseResponse<IPaginate<CarbonCreditResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tín chỉ carbon thành công",
                Data = credits
            };
        }
    }
}
