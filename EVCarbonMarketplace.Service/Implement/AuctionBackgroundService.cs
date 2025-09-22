using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class AuctionBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionBackgroundService> _logger;

        public AuctionBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AuctionBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<EvcarbonMarketplaceContext>>();
                        var bidService = scope.ServiceProvider.GetRequiredService<IBidService>();

                        var now = TimeUtil.GetCurrentSEATime();
                        _logger.LogInformation("[AuctionBackgroundService] Tick at {time}", now);

                        var listings = await unitOfWork.GetRepository<CarbonListing>()
                            .GetListAsync(predicate: x =>
                                x.Status == CarbonListingEnum.ListingStatus.Active.ToString()
                                && x.IsActive == true);

                        foreach (var listing in listings)
                        {
                           
                            if (listing.EndTime <= now)
                            {
                                await bidService.FinalizeAuction(listing.Id);
                                _logger.LogInformation($"[AuctionBackgroundService] Đã kết thúc đấu giá Listing {listing.Id}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[AuctionBackgroundService] Error while finalizing auctions");
                    }
                }

              
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
