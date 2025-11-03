using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.CarbonListing;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonListing;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class CarbonListingService : BaseService<CarbonListingService>, ICarbonListingService
    {
        public CarbonListingService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<CarbonListingService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public  async Task<BaseResponse<CarbonListingResponse>> Create(CarbonListingRequest request, CarbonListingEnum.ListingType? type)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            if (type == null) throw new BadHttpRequestException("Loại giao dịch không hợp lệ");
            var carbonCredit = await _unitOfWork.GetRepository<CarbonCredit>().SingleOrDefaultAsync(
                   predicate: x => x.Id == request.CarbonCreditId && x.IsActive == true
               ) ?? throw new NotFoundException("Không tìm thấy tín chỉ");
            var Creditlisting = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                predicate: x => x.CarbonCreditId == request.CarbonCreditId && x.IsActive == true && x.Status == CarbonListingEnum.ListingStatus.Active.ToString()
                );
            if (Creditlisting != null) throw new BadHttpRequestException("Tín chỉ đã được đăng bán");
            var sellerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == accountId && x.IsActive == true
            ) ?? throw new NotFoundException("Không tìm thấy ví");
            if (sellerWallet.CarbonUnit < (carbonCredit.Credits.Value))
            {
                throw new BadHttpRequestException("Bạn không đủ tín chỉ để đăng bán.");
            }
            sellerWallet.CarbonUnit -= carbonCredit.Credits;
            sellerWallet.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Wallet>().UpdateAsync(sellerWallet);
            carbonCredit.UpdateAt = TimeUtil.GetCurrentSEATime();
            carbonCredit.Status = CarbonCreditEnum.Listed.ToString();
            _unitOfWork.GetRepository<CarbonCredit>().UpdateAsync(carbonCredit);
            var CarbonLT = _mapper.Map<CarbonListing>(request);
            CarbonLT.AccountId = accountId;
            CarbonLT.Type = type.ToString();
            CarbonLT.Status = CarbonListingEnum.ListingStatus.Active.ToString();
            CarbonLT.CarbonCreditId = request.CarbonCreditId;

            await _unitOfWork.GetRepository<CarbonListing>().InsertAsync(CarbonLT);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if(!iSuccess) throw new Exception("Có lỗi trong quá trình tạo");

            var data = _mapper.Map<CarbonListingResponse>(CarbonLT);
            return new BaseResponse<CarbonListingResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đăng bán thành công",
                Data = data
            };
        }

        public async Task<BaseResponse<bool>> Delete(Guid id)
        {
            var listing = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tín chỉ");
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: x => x.CarbonListingId == id && x.IsActive == true
                );
            if (transaction != null) throw new NotFoundException("Tín chỉ đã được giao dịch, không thể xóa");
            listing.IsActive = false;
            listing.Status = CarbonListingEnum.ListingStatus.Cancelled.ToString();
            listing.DeleteAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<CarbonListing>().UpdateAsync(listing);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("Có lỗi trong quá trình xóa");
            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Xóa thành công",
                Data = true
            };


        }

        public async Task<BaseResponse<bool>> FinalizeListingExpiration(Guid listingId)
        {

            var listing = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                predicate: x => x.Id == listingId
                             && x.IsActive == true
                             && x.Status == CarbonListingEnum.ListingStatus.Active.ToString(),
                include: i => i.Include(c => c.CarbonCredit)
                               .ThenInclude(e => e.CarbonEmission)
                               .Include(a => a.Account)
            ) ?? throw new NotFoundException("Không tìm thấy bài đăng");

            if (listing.EndTime > TimeUtil.GetCurrentSEATime())
                throw new BadHttpRequestException("Bài đăng chưa hết hạn");

            listing.Status = CarbonListingEnum.ListingStatus.Expired.ToString();
            listing.IsActive = false;
            listing.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<CarbonListing>().UpdateAsync(listing);

            var carbonCredit = listing.CarbonCredit;
            carbonCredit.Status = CarbonCreditEnum.Available.ToString();
            carbonCredit.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<CarbonCredit>().UpdateAsync(carbonCredit);

            var result = await _unitOfWork.CommitAsync() > 0;
            if (!result) throw new Exception("Có lỗi trong quá trình hết hạn bài đăng");

            return new BaseResponse<bool>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Kết thúc bài đăng (Expired) thành công",
                Data = true
            };
        }

        public async Task<BaseResponse<IPaginate<CarbonListingManagerResponse>>> GetAll(int page, int size, CarbonListingEnum.ListingType? type, CarbonListingEnum.ListingStatus? status)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var now = TimeUtil.GetCurrentSEATime();
            _logger.LogInformation("now = {Now}", now);
            if (page <= 0 || size <= 0) throw new BadHttpRequestException("Trang và kích thước trang phải lớn hơn 0");
            var CreditListing = await _unitOfWork.GetRepository<CarbonListing>().GetPagingListAsync(
                
                selector: x => new CarbonListingManagerResponse
                {
                    Id = x.Id,
                    Credits = x.CarbonCredit.Credits ?? 0,
                    Price = x.Price,
                    Type = x.Type,
                    Status = x.Status,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,

                    SellerName = x.Account.FullName,
                    SellerAvatar = x.Account.AvatarUrl,

                    VehicleModel = x.CarbonCredit.CarbonEmission.ElectricVehicle.VehicleModel,
                    Brand = x.CarbonCredit.CarbonEmission.ElectricVehicle.Brand,
                    VehicleImage = x.CarbonCredit.CarbonEmission.ElectricVehicle.ImageUrl
                },

                predicate: x => x.IsActive == true
                && ((type == null || x.Type.ToLower() == type.ToString().ToLower())
                && (status == null || x.Status.ToLower() == status.ToString().ToLower())) && (account.Role != RoleEnum.CcBuyer.ToString()
                || (x.StartTime <= now && x.EndTime > now)),
                include: source => source.Include(x => x.Account).Include(x => x.CarbonCredit).ThenInclude(x => x.CarbonEmission).ThenInclude(x => x.ElectricVehicle),
                page: page,
                size: size
                );

            return new BaseResponse<IPaginate<CarbonListingManagerResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tín chỉ thành công",
                Data = CreditListing
            };

        }

        public async Task<BaseResponse<CarbonListingDetailResponse>> GetById(Guid id)
        {
            var CreditListing = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(

               selector: x => new CarbonListingDetailResponse
               {
                   Id = x.Id,
                   Credits = x.CarbonCredit.Credits ?? 0,
                   Price = x.Price,
                   Type = x.Type,
                   Status = x.Status,
                   StartTime = x.StartTime,
                   EndTime = x.EndTime,
                   CarbonCreditId = x.CarbonCredit.Id,
                   EmissionStart = x.CarbonCredit.CarbonEmission.PeriodStart.Value,
                   EmissionEnd = x.CarbonCredit.CarbonEmission.PeriodEnd.Value,
                   DistanceTravelled = x.CarbonCredit.CarbonEmission.DistanceTravelled.Value,
                   EnergyConsumed = x.CarbonCredit.CarbonEmission.EnergyConsumed.Value,
                   Co2Reduced = x.CarbonCredit.CarbonEmission.Co2reduced.Value,
                   BatteryCapacity = x.CarbonCredit.CarbonEmission.ElectricVehicle.BatteryCapacity,
                   Odometer = x.CarbonCredit.CarbonEmission.ElectricVehicle.Odometer,

                   SellerId = x.Account.Id,
                   SellerName = x.Account.FullName,
                   SellerAvatar = x.Account.AvatarUrl,

                   VehicleId = x.CarbonCredit.CarbonEmission.ElectricVehicle.Id,
                   VehicleType = x.CarbonCredit.CarbonEmission.ElectricVehicle.VehicleType.Name,
                   VehicleModel = x.CarbonCredit.CarbonEmission.ElectricVehicle.VehicleModel,
                   Brand = x.CarbonCredit.CarbonEmission.ElectricVehicle.Brand,
                   VehicleImage = x.CarbonCredit.CarbonEmission.ElectricVehicle.ImageUrl
               },

               predicate: x => x.IsActive == true && x.Id.Equals(id),
               include: source => source.Include(x => x.Account).Include(x => x.CarbonCredit).ThenInclude(x => x.CarbonEmission).ThenInclude(x => x.ElectricVehicle)

               );

            return new BaseResponse<CarbonListingDetailResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy tín chỉ thành công",
                Data = CreditListing
            };

        }

        public async Task<BaseResponse<IPaginate<CarbonListingManagerResponse>>> GetMyListing(int page, int size, CarbonListingEnum.ListingType? type, CarbonListingEnum.ListingStatus? status)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            if (page <= 0 || size <= 0) throw new BadHttpRequestException("Trang và kích thước trang phải lớn hơn 0");

            var CreditListing = await _unitOfWork.GetRepository<CarbonListing>().GetPagingListAsync(

                selector: x => new CarbonListingManagerResponse
                {
                    Id = x.Id,
                    Credits = x.CarbonCredit.Credits ?? 0,
                    Price = x.Price,
                    Type = x.Type,
                    Status = x.Status,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,


                    SellerName = x.Account.FullName,
                    SellerAvatar = x.Account.AvatarUrl,

                    VehicleModel = x.CarbonCredit.CarbonEmission.ElectricVehicle.VehicleModel,
                    Brand = x.CarbonCredit.CarbonEmission.ElectricVehicle.Brand,
                    VehicleImage = x.CarbonCredit.CarbonEmission.ElectricVehicle.ImageUrl



                },

                predicate: x => x.IsActive == true && x.AccountId == accountId && (type == null || x.Type == type.ToString()) && (status == null || x.Status == status.ToString()),
                include: source => source.Include(x => x.Account).Include(x => x.CarbonCredit).ThenInclude(x => x.CarbonEmission).ThenInclude(x => x.ElectricVehicle),
                page: page,
                size: size
                );

            return new BaseResponse<IPaginate<CarbonListingManagerResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách tín chỉ thành công",
                Data = CreditListing
            };
        }

        public async Task<BaseResponse<CarbonListingResponse>> Update(Guid id, CarbonListingUpdateRequest request)
        {
            var carbonlisting = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                predicate: x => x.Id == id && x.IsActive == true,
                include: s => s.Include(x => x.CarbonCredit).ThenInclude(x => x.CarbonEmission).ThenInclude(x => x.ElectricVehicle)
                ) ?? throw new NotFoundException("Không tìm thấy tín chỉ đăng bán");
            var transaction = await _unitOfWork.GetRepository<Transaction>().SingleOrDefaultAsync(
                predicate: x => x.CarbonListingId == id && x.IsActive == true
                );
            if (transaction != null) throw new NotFoundException("Tín chỉ đã được giao dịch, không thể chỉnh sửa");

            carbonlisting.Price = request.Price;
            carbonlisting.EndTime = request.EndTime;
            carbonlisting.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<CarbonListing>().UpdateAsync(carbonlisting);
            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("Có lỗi trong quá trình chỉnh sửa");

            return new BaseResponse<CarbonListingResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Chỉnh sửa thành công",
                Data = new CarbonListingResponse
                {
                    Id = carbonlisting.Id,
                    CarbonCreditId = carbonlisting.CarbonCredit.Id,
                    Price = carbonlisting.Price,
                    Type = carbonlisting.Type,
                    Status = carbonlisting.Status,
                    StartTime = carbonlisting.StartTime,
                    EndTime = carbonlisting.EndTime,
                    SellerId = carbonlisting.AccountId.Value,
                }
            };

        }
    }
}
