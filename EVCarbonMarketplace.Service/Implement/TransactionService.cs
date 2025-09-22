using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Transaction;
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
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;

namespace EVCarbonMarketplace.Service.Implement
{
    public class TransactionService : BaseService<TransactionService>, ITransactionService
    {
        public TransactionService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<TransactionService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<IPaginate<TransactionUserResponse>>> GetAll(int page, int size, TransactionEnum? type, TransactionStatusEnum? status)
        {
            var transactions = await _unitOfWork.GetRepository<Transaction>().GetPagingListAsync(
                selector: s => new TransactionUserResponse
                {
                    Id = s.Id,
                    CarbonListingId = s.CarbonListingId,
                    CarbonCreditId = s.CarbonListing.CarbonCreditId,
                    Type = s.Type,
                    Status = s.Status,
                    Amount = s.Amount,
                    CreateAt = s.CreateAt,
                    Description = s.Description
                },


                predicate: x => (type == null || x.Type == type.ToString()) && (status == null || x.Status == status.ToString()) && x.IsActive == true,
                include: i => i.Include(w => w.Wallet).Include(b => b.Buyer).Include(s => s.Seller).Include(l => l.CarbonListing).ThenInclude(c => c.CarbonCredit).ThenInclude(e => e.CarbonEmission),
                orderBy: o => o.OrderByDescending(c => c.CreateAt),
                page: page,
                size: size
                );
            if (transactions == null || !transactions.Items.Any()) throw new NotFoundException("Không tìm thấy giao dịch");
            return new BaseResponse<IPaginate<TransactionUserResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách giao dịch thành công",
                Data = transactions
            };
        }

        public async Task<BaseResponse<IPaginate<TransactionUserResponse>>> GetMyTransaction(int page, int size, TransactionEnum? type, TransactionStatusEnum? status)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                );
            if (account == null) throw new NotFoundException("Không tìm thấy tài khoản");


            var transactions = await _unitOfWork.GetRepository<Transaction>().GetPagingListAsync(
        selector: s => new TransactionUserResponse
        {
            Id = s.Id,
            CarbonListingId = s.CarbonListingId,
            CarbonCreditId = s.CarbonListing.CarbonCreditId,
            Type = s.Type,
            Status = s.Status,
            Amount = s.Amount,
            CreateAt = s.CreateAt,
            Description = s.Description
        },


        predicate: x => (type == null || x.Type == type.ToString()) && (status == null || x.Status == status.ToString()) && x.IsActive == true  && x.Wallet.AccountId == accountId,
        include: i => i.Include(w => w.Wallet).Include(b => b.Buyer).Include(s => s.Seller).Include(l => l.CarbonListing).ThenInclude(c => c.CarbonCredit).ThenInclude(e => e.CarbonEmission),
        orderBy: o => o.OrderByDescending(c => c.CreateAt),
        page: page,
        size: size
        );
            if (transactions == null || !transactions.Items.Any()) throw new NotFoundException("Không tìm thấy giao dịch");
            return new BaseResponse<IPaginate<TransactionUserResponse>>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy danh sách giao dịch thành công",
                Data = transactions
            };

        }

        public async Task<BaseResponse<TransactionResponse>> Purchase(Guid listingId)
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true
                );
            if (account == null) throw new NotFoundException("Không tìm thấy tài khoản");

            var listing = await _unitOfWork.GetRepository<CarbonListing>().SingleOrDefaultAsync(
                predicate: x => x.Id == listingId && x.IsActive == true && x.Status == CarbonListingEnum.ListingStatus.Active.ToString(),
                include: i => i.Include(c => c.CarbonCredit).ThenInclude(e => e.CarbonEmission).Include(a => a.Account)
            );
            if (listing == null) throw new NotFoundException("Không tìm thấy tín chỉ");

            var buyerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == accountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy ví người mua");

            var sellerWallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == listing.AccountId && x.IsActive == true
                ) ?? throw new NotFoundException("Không tìm thấy ví người bán");
            if (buyerWallet.Cash < listing.Price)
            {
                var amountNeeded = listing.Price- buyerWallet.Cash;
                throw new BadHttpRequestException(
                    $"Số dư trong ví không đủ để thực hiện giao dịch. Vui lòng nạp thêm {amountNeeded:N0} VND."
                );
            }
            buyerWallet.Cash -= listing.Price;
            buyerWallet.UpdateAt  = TimeUtil.GetCurrentSEATime();
            buyerWallet.CarbonUnit += listing.CarbonCredit.Credits;
            sellerWallet.Cash += listing.Price;
            sellerWallet.CarbonUnit -= listing.CarbonCredit.Credits;
            sellerWallet.UpdateAt = TimeUtil.GetCurrentSEATime();
            _unitOfWork.GetRepository<Wallet>().UpdateAsync(buyerWallet);
            _unitOfWork.GetRepository<Wallet>().UpdateAsync(sellerWallet);

            listing.Status = CarbonListingEnum.ListingStatus.Sold.ToString();
            listing.UpdateAt = TimeUtil.GetCurrentSEATime();
             _unitOfWork.GetRepository<CarbonListing>().UpdateAsync(listing);
            var credit = listing.CarbonCredit
             ?? throw new NotFoundException("Không tìm thấy tín chỉ");
            credit.AccountId = accountId;
            credit.UpdateAt = TimeUtil.GetCurrentSEATime();


            var transaction = new Transaction
            {
                WalletId = buyerWallet.Id,
                Id = Guid.NewGuid(),
                BuyerId = accountId,
                SellerId = listing.AccountId,
                CarbonListingId = listing.Id,
                Amount = listing.Price,
                CreateAt = TimeUtil.GetCurrentSEATime(),
                Description = "Mua tín chỉ carbon",
                Type = TransactionEnum.Purchase.ToString(),
                Status = "Success",
                IsActive = true          
            };
            await _unitOfWork.GetRepository<Transaction>().InsertAsync(transaction);
            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccess) throw new Exception("Có lỗi trong quá trình giao dịch");
            var response = new TransactionResponse
            {
                Id = transaction.Id,
                ListingId = listing.Id,
                Status = transaction.Status,
                Amount = transaction.Amount.Value,
                Credits = listing.CarbonCredit.Credits.Value,
                CreateAt = transaction.CreateAt.Value,

                Type = transaction.Type,
                Price = listing.Price,

                BuyerId = accountId.Value,
                BuyerName = account.FullName,
                BuyerAvatar = account.AvatarUrl,

                SellerId = listing.AccountId.Value,
                SellerName = listing.Account.FullName,
                SellerAvatar = listing.Account.AvatarUrl,

                CarbonCreditId = listing.CarbonCredit.Id,
                EmissionStart = listing.CarbonCredit.CarbonEmission.PeriodStart.Value,
                EmissionEnd = listing.CarbonCredit.CarbonEmission.PeriodEnd.Value,
                Co2Reduced = listing.CarbonCredit.CarbonEmission.Co2reduced.Value,
            };
            return new BaseResponse<TransactionResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Giao dịch thành công",
                Data =  response
            };
        }
    }
}
