using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.Bid;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Bid;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class BidController : BaseController<BidController>
    {
        private readonly IBidService _bidService;

        public BidController(ILogger<BidController> logger ,IBidService bidService) : base(logger)
        {
           _bidService = bidService;
        }

        [HttpPost(ApiEndPointConstant.Bid.PlaceBid)]
        [ProducesResponseType(typeof(BaseResponse<BidResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<BidResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> PlaceBid([FromBody] BidRequest request)
        {
            var response = await _bidService.PlaceBid(request);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost(ApiEndPointConstant.Bid.FinalizeAuction)]
        [ProducesResponseType(typeof(BaseResponse<BidResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<BidResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> FinalizeAuction([FromQuery] Guid listingId)
        {
            var response = await _bidService.FinalizeAuction(listingId);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpGet(ApiEndPointConstant.Bid.GetCurrentBid)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<BidResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<BidResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetCurrentBid([FromQuery] Guid listingId ,[FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _bidService.GetCurrentBid(page, size, listingId);
            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}
