using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Bid;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Identity.Client;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.Bid;

namespace EVCarbonMarketplace.Service.Implement
{
    public class BidService : BaseService<BidService>, IBidService
    {
        public BidService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<BidService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<BidResponse>> FinalizeAuction(Guid listingId)
        {
            var listing = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                predicate: x => x.Id == listingId && x.IsActive == true && x.Status == CarbonListingEnum.ListingStatus.Active.ToString(),
                include: i => i.Include(c => c.CarbonCredit).ThenInclude(e => e.CarbonEmission).Include(a => a.Account)
                ) ?? throw new NotFoundException("Không tìm thấy tín chỉ");
            if (listing.Type != CarbonListingEnum.ListingType.Auction.ToString())
                throw new BadHttpRequestException("Listing này không phải đấu giá");

            var highestBid = (await _unitOfWork.GetRepository<Bid>().GetListAsync(
              predicate: x => x.CarbonListingId == listingId && x.IsActive == true,
              orderBy: q => q.OrderByDescending(x => x.Price)
          )).FirstOrDefault();
            var sellerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
              predicate: x => x.AccountId == listing.AccountId && x.IsActive == true
              ) ?? throw new NotFoundException("Không tìm thấy ví người bán");

            if (highestBid == null)
            {
                listing.Status = CarbonListingEnum.ListingStatus.Expired.ToString();
                listing.UpdateAt = TimeUtil.GetCurrentSEATime();
                _unitOfWork.GetRepository<CarbonListing>().UpdateAsync(listing);


                sellerWallet.CarbonUnit += listing.CarbonCredit.Credits.Value;
                sellerWallet.UpdateAt = TimeUtil.GetCurrentSEATime();
                _unitOfWork.GetRepository<Wallet>().UpdateAsync(sellerWallet);
                var iSuccesss = await _unitOfWork.CommitAsync() > 0;
                if (!iSuccesss) throw new Exception("Có lỗi trong quá trình kết thúc đấu giá");
                return new BaseResponse<BidResponse>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Đấu giá kết thúc, không có ai đặt giá",
                    Data = null
                };
            }
            //phí
            var feeSetting = await _unitOfWork.GetRepository<SystemSetting>()
             .SingleOrDefaultAsync(predicate: s => s.Key == "TransactionFeeRate");
            decimal feeRate = feeSetting != null ? decimal.Parse(feeSetting.Value) : 0;
            decimal feeAmount = (highestBid.Price.Value * feeRate) / 100;
            decimal sellerReceive = highestBid.Price.Value - feeAmount;

            //Cập nhật ví người bán và người mua
            var buyerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                 predicate: x => x.AccountId == highestBid.AccountId && x.IsActive == true
                 ) ?? throw new NotFoundException("Không tìm thấy ví người mua");
            buyerWallet.CarbonUnit += listing.CarbonCredit.Credits.Value;
            buyerWallet.UpdateAt = TimeUtil.GetCurrentSEATime();

            sellerWallet.Cash += sellerReceive;
            sellerWallet.UpdateAt = TimeUtil.GetCurrentSEATime();
            listing.Status = CarbonListingEnum.ListingStatus.Sold.ToString();
            listing.UpdateAt = TimeUtil.GetCurrentSEATime();
            highestBid.Status = BidEnum.Winner.ToString();
            highestBid.UpdateAt = TimeUtil.GetCurrentSEATime();
            var otherBids = await _unitOfWork.GetRepository<Bid>()
                .GetListAsync(predicate: x => x.CarbonListingId == listingId && x.Id != highestBid.Id);
            foreach (var bid in otherBids)
            {
                bid.Status = BidEnum.Loser.ToString();
                bid.UpdateAt = TimeUtil.GetCurrentSEATime();
            }
            var transactions = await _unitOfWork.GetRepository<Transaction>()
                .GetListAsync(predicate: x => x.CarbonListingId == listingId && x.Type == TransactionEnum.Auction.ToString() && x.Status == "Hold");
            var loserIds = transactions
             .Where(t => t.BuyerId != highestBid.AccountId)
             .Select(t => t.BuyerId)
             .Distinct()
             .ToList();

            var loserWallets = await _unitOfWork.GetRepository<Wallet>()
                .GetListAsync(predicate: w => loserIds.Contains(w.AccountId) && w.IsActive == true);

            foreach (var t in transactions)
            {
                if (t.BuyerId == highestBid.AccountId)
                {
                    t.Status = TransactionStatusEnum.Success.ToString();
                    t.FeeRate = feeRate;    
                }
                else
                {
                    t.Status = TransactionStatusEnum.Fail.ToString();

                    var loserWallet = loserWallets.FirstOrDefault(w => w.AccountId == t.BuyerId);
                    if (loserWallet != null)
                    {
                        loserWallet.Cash += t.Amount;
                        loserWallet.UpdateAt = TimeUtil.GetCurrentSEATime();
                    }
                }
                t.UpdateAt = TimeUtil.GetCurrentSEATime();
            }
            var sellerTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                WalletId = sellerWallet.Id,
                BuyerId = highestBid.AccountId,
                SellerId = listing.AccountId,
                CarbonListingId = listing.Id,
                Amount = sellerReceive,
                Description = "Nhận tiền bán đấu giá tín chỉ carbon",
                CreateAt = TimeUtil.GetCurrentSEATime(),
                Type = TransactionEnum.Sale.ToString(),
                Status = TransactionStatusEnum.Success.ToString(),
                FeeRate = feeRate,
                IsActive = true
            };
            await _unitOfWork.GetRepository<Transaction>().InsertAsync(sellerTransaction);
            listing.CarbonCredit.AccountId = highestBid.AccountId;
            listing.CarbonCredit.UpdateAt = TimeUtil.GetCurrentSEATime();
            listing.CarbonCredit.Status = CarbonCreditEnum.Available.ToString();
            _unitOfWork.GetRepository<CarbonCredit>().UpdateAsync(listing.CarbonCredit);

            _unitOfWork.GetRepository<Wallet>().UpdateRange(
                 new[] { buyerWallet, sellerWallet }.Concat(loserWallets)
             );
            _unitOfWork.GetRepository<CarbonListing>().UpdateAsync(listing);
            _unitOfWork.GetRepository<Bid>().UpdateRange(new[] { highestBid }.Concat(otherBids));
            _unitOfWork.GetRepository<Transaction>().UpdateRange(transactions);


            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("Có lỗi trong quá trình kết thúc đấu giá");
            return new BaseResponse<BidResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đấu giá kết thúc, giao dịch thành công",
                Data = new BidResponse
                {
                    Id = highestBid.Id,
                    AccountId = highestBid.AccountId,
                    CarbonListingId = listing.Id,
                    BidTime = highestBid.BidTime,
                    Price = highestBid.Price,
                    CreateAt = highestBid.CreateAt,
                    Status = highestBid.Status
                }
            };
        }

        public async Task<BaseResponse<IPaginate<BidResponse>>> GetCurrentBid(int page, int size, Guid listingId)
        {
            var bids = await _unitOfWork.GetRepository<Bid>().GetPagingListAsync(
                selector: s => new BidResponse
                {
                    Id = s.Id,
                    AccountId = s.AccountId,
                    CarbonListingId = s.CarbonListingId,
                    BidTime = s.BidTime,
                    Price = s.Price,
                    Status = s.Status,
                    CreateAt = s.CreateAt,
                },

                predicate: x => x.CarbonListingId == listingId && x.IsActive == true,
                orderBy: q => q.OrderByDescending(b => b.Price),
                page: page,
                size: size
                );
            if (!bids.Items.Any()) throw new NotFoundException("Chưa có ai đấu giá");

            return new BaseResponse<IPaginate<BidResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách giá thầu thành công",
                Data = bids

            };
        }

        public async Task<BaseResponse<BidResponse>> PlaceBid(BidRequest request)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true && x.DeleteAt == null

                ) ?? throw new NotFoundException("Không tìm thấy tài khoản");

            var listing = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                  predicate: x => x.Id == request.CarbonListingId && x.IsActive == true && x.Status == CarbonListingEnum.ListingStatus.Active.ToString(),
                  include: i => i.Include(c => c.CarbonCredit).ThenInclude(e => e.CarbonEmission).Include(a => a.Account)
              );
            if (listing == null) throw new NotFoundException("Không tìm thấy tín chỉ");
            var now = TimeUtil.GetCurrentSEATime();
            if (listing.EndTime <= now)
            {
                throw new BadHttpRequestException("Đấu giá đã kết thúc, không thể đặt giá nữa.");
            }

            var buyerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy ví người mua");
            if (buyerWallet.Cash < request.Price)
            {
                var amountNeeded = request.Price - buyerWallet.Cash;
                throw new BadHttpRequestException(
                    $"Số dư trong ví không đủ để thực hiện đấu giá. Vui lòng nạp thêm {amountNeeded:N0} VND."
                );
            }

            var highestBid = await _unitOfWork.GetRepository<Bid>()
                .GetListAsync(predicate: x => x.CarbonListingId == request.CarbonListingId && x.IsActive == true);
            //if (!highestBid.Any() && request.Price < listing.Price)
            //{
            //    throw new BadHttpRequestException($"Giá khởi điểm là {listing.Price:N0}, bạn phải đặt giá cao hơn hoặc bằng.");
            //}
            if (highestBid.Any() && request.Price <= highestBid.Max(x => x.Price))
            {
                throw new BadHttpRequestException("Giá bạn đặt phải cao hơn giá hiện tại.");
            }
            if (listing.AccountId == accountId)
            {
                throw new BadHttpRequestException("Bạn không thể đấu giá tín chỉ của chính mình.");
            }
            var previousBid = highestBid
               .Where(x => x.AccountId == accountId)
               .OrderByDescending(x => x.Price)
               .FirstOrDefault();

            var previousPrice = previousBid?.Price ?? 0m;
            var requiredHold = request.Price - previousPrice;



            buyerWallet.Cash -= requiredHold;
            buyerWallet.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Wallet>().UpdateAsync(buyerWallet);
            var bid = new Bid
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                CarbonListingId = request.CarbonListingId,
                BidTime = now,
                Price = request.Price,
                Status = BidEnum.Pending.ToString(),
                IsActive = true,
                CreateAt = TimeUtil.GetCurrentSEATime()
            };
            await _unitOfWork.GetRepository<Bid>().InsertAsync(bid);
            var holdTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                WalletId = buyerWallet.Id,
                BuyerId = accountId,
                SellerId = listing.AccountId,
                CarbonListingId = listing.Id,
                Amount = requiredHold,
                Description = previousPrice > 0
                ? $"Nâng giá từ {previousPrice:N0} lên {request.Price:N0}"
                : "Đặt giá đấu giá",
                CreateAt = now,
                Type = TransactionEnum.Auction.ToString(),
                Status = "Hold",
                IsActive = true,
                FeeRate = 0
            };
            await _unitOfWork.GetRepository<Transaction>().InsertAsync(holdTransaction);

            var iSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!iSuccess) throw new Exception("Có lỗi trong quá trình đấu giá");

            return new BaseResponse<BidResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đặt giá thành công",
                Data = new BidResponse
                {
                    AccountId = accountId,
                    CarbonListingId = request.CarbonListingId,
                    BidTime = bid.BidTime,
                    Price = bid.Price,
                    CreateAt = bid.CreateAt,
                    Id = bid.Id,
                    Status = bid.Status,
                }
            };
        }
    }
}
