using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Analytic;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Google.Analytics.Data.V1Beta;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class AnalyticsService : BaseService<AnalyticsService>, IAnalyticsService
    {
        private readonly string _serviceAccountKeyPath;
        private readonly string _propertyId;
        public AnalyticsService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<AnalyticsService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _serviceAccountKeyPath = Path.Combine(AppContext.BaseDirectory, configuration["GoogleAnalytics:ServiceAccountKeyPath"]);
            _propertyId = configuration["GoogleAnalytics:PropertyId"];
        }

        public async Task<BaseResponse<FinanceStatsResponse>> GetFinanceStats()
        {
            try
            {
                // 1. Lấy tất cả transaction success
                var transactions = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                    predicate: t => t.IsActive == true && t.Status == "Success"
                );

                // 2. Phân loại
                var purchaseTransactions = transactions
                    .Where(t => t.Type == TransactionEnum.Purchase.ToString())
                    .ToList();

                // Auction: chỉ lấy Winner (bid cuối cùng mỗi listing)
                var auctionTransactions = transactions
                    .Where(t => t.Type == TransactionEnum.Auction.ToString())
                    .GroupBy(t => t.CarbonListingId)
                    .Select(g => g.OrderByDescending(t => t.CreateAt).First())
                    .ToList();

                // 3. Danh sách hợp lệ để tính doanh thu
                var validTransactions = purchaseTransactions.Concat(auctionTransactions).ToList();

                // 4. Tổng Deposit / Withdraw
                var totalDeposit = transactions
                    .Where(t => t.Type == TransactionEnum.Deposit.ToString())
                    .Sum(t => t.Amount) ?? 0;

                var totalWithdraw = transactions
                    .Where(t => t.Type == TransactionEnum.Withdraw.ToString())
                    .Sum(t => t.Amount) ?? 0;

                // 5. Tổng doanh thu
                var totalRevenue = validTransactions
                    .Sum(t => (t.Amount ?? 0) * (t.FeeRate ?? 0) / 100);

                // 6. Thống kê theo ngày
                // Lấy tất cả ngày có Deposit / Withdraw hoặc Revenue
                var depositWithdrawDates = transactions
                    .Where(x => x.CreateAt.HasValue &&
                                (x.Type == TransactionEnum.Deposit.ToString() ||
                                 x.Type == TransactionEnum.Withdraw.ToString()))
                    .Select(x => x.CreateAt.Value.Date);

                var revenueDates = validTransactions
                    .Where(x => x.CreateAt.HasValue)
                    .Select(x => x.CreateAt.Value.Date);

                var allDates = depositWithdrawDates
                    .Union(revenueDates)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                var byDate = allDates
                    .Select(d => new FinanceDailyStats
                    {
                        Date = d,
                        Deposit = transactions
                            .Where(x => x.Type == TransactionEnum.Deposit.ToString()
                                        && x.CreateAt.HasValue
                                        && x.CreateAt.Value.Date == d)
                            .Sum(x => x.Amount) ?? 0,

                        Withdraw = transactions
                            .Where(x => x.Type == TransactionEnum.Withdraw.ToString()
                                        && x.CreateAt.HasValue
                                        && x.CreateAt.Value.Date == d)
                            .Sum(x => x.Amount) ?? 0,

                        Revenue = validTransactions
                            .Where(x => x.CreateAt.HasValue
                                        && x.CreateAt.Value.Date == d)
                            .Sum(x => (x.Amount ?? 0) * (x.FeeRate ?? 0) / 100)
                    })
                    .ToList();

                // 7. Trả response
                var response = new FinanceStatsResponse
                {
                    TotalDeposit = totalDeposit,
                    TotalWithdraw = totalWithdraw,
                    TotalRevenue = totalRevenue,
                    ByDate = byDate
                };

                return new BaseResponse<FinanceStatsResponse>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Lấy thống kê tài chính thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Analytics] Lỗi khi thống kê tài chính");

                return new BaseResponse<FinanceStatsResponse>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Đã xảy ra lỗi khi xử lý dữ liệu",
                    Data = null
                };
            }
        }


        public async Task<BaseResponse<string>> GetRealtimeUsers()
        {
             try
            {
                var credential = GoogleCredential.FromFile(_serviceAccountKeyPath)
                    .CreateScoped("https://www.googleapis.com/auth/analytics.readonly");

                var client = new BetaAnalyticsDataClientBuilder { Credential = credential }.Build();

                var request = new RunRealtimeReportRequest
                {
                    Property = $"properties/{_propertyId}",
                    Metrics = { new Metric { Name = "activeUsers" } }
                };

                var response = await client.RunRealtimeReportAsync(request);

                string result = "0";
                if (response?.Rows != null && response.Rows.Count > 0 && response.Rows[0].MetricValues.Count > 0)
                {
                    result = response.Rows[0].MetricValues[0].Value;
                }

                return new BaseResponse<string>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Lấy dữ liệu người dùng đang hoạt động",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time GA users");
                return new BaseResponse<string>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Lỗi khi lấy người dùng",
                    Data = null
                };
            }
        }

        public async Task<BaseResponse<List<AnalyticsUserDailyResponse>>> GetRegisteredUsersByDay()
        {
            try
            {
                var rawList = await _unitOfWork.GetRepository<Account>()
                    .GetListAsync(
                        selector: g => new
                        {
                            Date = g.CreateAt!.Value.Date
                        },
                        predicate: u => u.CreateAt.HasValue &&
                              (u.Role == RoleEnum.EvOwner.ToString() || u.Role == RoleEnum.CcBuyer.ToString())
                    );

                var grouped = rawList
                    .GroupBy(x => x.Date)
                    .Select(g => new AnalyticsUserDailyResponse
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return new BaseResponse<List<AnalyticsUserDailyResponse>>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Lấy thống kê đăng ký người dùng theo ngày thành công",
                    Data = grouped
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Analytics] Lỗi khi thống kê đăng ký người dùng theo ngày");

                return new BaseResponse<List<AnalyticsUserDailyResponse>>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Đã xảy ra lỗi khi xử lý dữ liệu",
                    Data = null
                };
            }
        }

        public async Task<BaseResponse<TransactionStatsResponse>> GetTransactionStats()
        {
            try
            {
                // 1. Lấy danh sách Listing để thống kê
                var listings = await _unitOfWork.GetRepository<CarbonListing>().GetListAsync();

                // 2. Lấy tất cả Transaction thành công (Purchase + Auction)
                var allTransactions = await _unitOfWork.GetRepository<Transaction>().GetListAsync(
                    predicate: t => t.IsActive == true && t.Status == "Success" &&
                                    (t.Type == TransactionEnum.Purchase.ToString() ||
                                     t.Type == TransactionEnum.Auction.ToString()),
                    include: i => i.Include(t => t.CarbonListing)
                                   .ThenInclude(l => l.CarbonCredit)
                );

                // 3. Phân loại
                var purchaseTransactions = allTransactions
                    .Where(t => t.Type == TransactionEnum.Purchase.ToString())
                    .ToList();

                // Auction: gom theo listing, chỉ lấy Winner với tổng amount
                var auctionTransactions = allTransactions
                    .Where(t => t.Type == TransactionEnum.Auction.ToString())
                    .GroupBy(t => t.CarbonListingId)
                    .Select(g =>
                    {
                        var winner = g.OrderByDescending(x => x.CreateAt).First();
                        var winnerId = winner.BuyerId;

                        // Tổng tất cả bid của Winner trong listing này
                        var totalWinnerAmount = g
                            .Where(x => x.BuyerId == winnerId)
                            .Sum(x => x.Amount ?? 0);

                        // Tạo transaction đại diện cho phiên đấu giá
                        var finalTransaction = new Transaction
                        {
                            Id = winner.Id,
                            BuyerId = winner.BuyerId,
                            SellerId = winner.SellerId,
                            CarbonListingId = winner.CarbonListingId,
                            CarbonListing = winner.CarbonListing,
                            Amount = totalWinnerAmount,
                            FeeRate = winner.FeeRate,
                            CreateAt = winner.CreateAt,
                            Type = winner.Type,
                            Status = winner.Status,
                            IsActive = winner.IsActive
                        };
                        return finalTransaction;
                    })
                    .ToList();

                // 4. Danh sách hợp lệ để tính thống kê
                var validTransactions = purchaseTransactions.Concat(auctionTransactions).ToList();

                // 5. Thống kê tổng quát
                var totalListings = listings.Count;
                var activeListings = listings.Count(l => l.Status == CarbonListingEnum.ListingStatus.Active.ToString());
                var soldListings = listings.Count(l => l.Status == CarbonListingEnum.ListingStatus.Sold.ToString());
                var expiredListings = listings.Count(l => l.Status == CarbonListingEnum.ListingStatus.Expired.ToString());
                var cancelledListings = listings.Count(l => l.Status == CarbonListingEnum.ListingStatus.Cancelled.ToString());

                var totalTransactions = validTransactions.Count;
                var fixedPriceTransactions = purchaseTransactions.Count;
                var auctionSuccessTransactions = auctionTransactions.Count;

                var totalCreditsSold = validTransactions.Sum(t =>
                    t.CarbonListing != null && t.CarbonListing.CarbonCredit != null
                        ? (t.CarbonListing.CarbonCredit.Credits ?? 0)
                        : 0);

                var avgPrice = validTransactions.Any() ? validTransactions.Average(t => t.Amount ?? 0) : 0;
                var minPrice = validTransactions.Any() ? validTransactions.Min(t => t.Amount ?? 0) : 0;
                var maxPrice = validTransactions.Any() ? validTransactions.Max(t => t.Amount ?? 0) : 0;

                // 6. Thống kê theo ngày
                var byDate = validTransactions
                    .Where(t => t.CreateAt.HasValue)
                    .GroupBy(t => t.CreateAt.Value.Date)
                    .Select(g => new TransactionDailyStats
                    {
                        Date = g.Key,
                        Transactions = g.Count(),
                        CreditsSold = g.Sum(t =>
                            t.CarbonListing != null && t.CarbonListing.CarbonCredit != null
                                ? (t.CarbonListing.CarbonCredit.Credits ?? 0)
                                : 0),
                        AvgPrice = g.Average(t => t.Amount ?? 0)
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                // 7. Trả kết quả
                var response = new TransactionStatsResponse
                {
                    TotalListings = totalListings,
                    ActiveListings = activeListings,
                    SoldListings = soldListings,
                    ExpiredListings = expiredListings,
                    CancelledListings = cancelledListings,
                    TotalTransactions = totalTransactions,
                    FixedPriceTransactions = fixedPriceTransactions,
                    AuctionTransactions = auctionSuccessTransactions,
                    TotalCreditsSold = totalCreditsSold,
                    AvgPrice = avgPrice,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    ByDate = byDate
                };

                return new BaseResponse<TransactionStatsResponse>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Lấy thống kê giao dịch thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Analytics] Lỗi khi thống kê giao dịch");
                return new BaseResponse<TransactionStatsResponse>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Đã xảy ra lỗi khi xử lý dữ liệu",
                    Data = null
                };
            }
        }




        public async Task<BaseResponse<AnalyticsUsersResponse>> GetUsers()
        {
            try
            {
                var credential = GoogleCredential.FromFile(_serviceAccountKeyPath)
                    .CreateScoped("https://www.googleapis.com/auth/analytics.readonly");

                var client = new BetaAnalyticsDataClientBuilder { Credential = credential }.Build();

                var request = new RunReportRequest
                {
                    Property = $"properties/{_propertyId}",
                    DateRanges = { new DateRange { StartDate = "7daysAgo", EndDate = "today" } },
                    Metrics =
            {
                new Metric { Name = "activeUsers" },
                new Metric { Name = "sessions" }
            }
                };

                var response = await client.RunReportAsync(request);

                string users = "0";
                string sessions = "0";

                if (response.Rows != null && response.Rows.Count > 0)
                {
                    var firstRow = response.Rows[0];
                    if (firstRow.MetricValues.Count > 0) users = firstRow.MetricValues[0].Value;
                    if (firstRow.MetricValues.Count > 1) sessions = firstRow.MetricValues[1].Value;
                }

                var data = new AnalyticsUsersResponse
                {
                    Users = users,
                    Sessions = sessions
                };

                return new BaseResponse<AnalyticsUsersResponse>
                {
                    Status = StatusCodes.Status200OK.ToString(),
                    Message = "Lấy dữ liệu người dùng trong 7 ngày",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GA users");
                return new BaseResponse<AnalyticsUsersResponse>
                {
                    Status = StatusCodes.Status500InternalServerError.ToString(),
                    Message = "Lỗi khi lấy dữ liệu người dùng",
                    Data = null
                };
            }
        }

    }
}
