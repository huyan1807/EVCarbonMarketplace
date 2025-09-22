using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.Bid;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Bid;
using EVCarbonMarketplace.Model.Payload.Response.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IBidService
    {
        Task<BaseResponse<BidResponse>> PlaceBid(BidRequest request);
        Task<BaseResponse<BidResponse>> FinalizeAuction(Guid listingId);
        Task<BaseResponse<IPaginate<BidResponse>>> GetCurrentBid(int page , int size,Guid listingId);
    }
}
