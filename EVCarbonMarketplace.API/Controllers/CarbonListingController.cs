using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response.CarbonCredit;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EVCarbonMarketplace.Model.Payload.Response.CarbonListing;
using EVCarbonMarketplace.Model.Payload.Request.CarbonListing;
using EVCarbonMarketplace.Model.Enum;
using Microsoft.AspNetCore.Authorization;

namespace EVCarbonMarketplace.API.Controllers
{

    public class CarbonListingController : BaseController<CarbonListingController>
    {
        private readonly ICarbonListingService _carbonListingService;
        public CarbonListingController(ILogger<CarbonListingController> logger , ICarbonListingService carbonListingService) : base(logger)
        {
            _carbonListingService = carbonListingService;
        }
        [Authorize(Roles = "EvOwner")]
        [HttpPost(ApiEndPointConstant.CarbonListing.CreateSellListing)]
        [ProducesResponseType(typeof(BaseResponse<CarbonListingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonListingResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateSellListing([FromBody] CarbonListingRequest request , [FromQuery] CarbonListingEnum.ListingType? type = null)
        {
            var response = await _carbonListingService.Create(request, type);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpGet(ApiEndPointConstant.CarbonListing.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonListingManagerResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonListingManagerResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll([FromQuery] CarbonListingEnum.ListingType? type =null , [FromQuery] CarbonListingEnum.ListingStatus? status = null ,[FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _carbonListingService.GetAll(page, size, type, status);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles = "Admin,Cva")]
        [HttpDelete(ApiEndPointConstant.CarbonListing.Delete)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _carbonListingService.Delete(id);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet(ApiEndPointConstant.CarbonListing.GetMyListings)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonListingManagerResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonListingManagerResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetMyListings([FromQuery] CarbonListingEnum.ListingType? type = null, [FromQuery] CarbonListingEnum.ListingStatus? status = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _carbonListingService.GetMyListing(page, size, type, status);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpGet(ApiEndPointConstant.CarbonListing.GetById)]
        [ProducesResponseType(typeof(BaseResponse<CarbonListingDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonListingDetailResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _carbonListingService.GetById(id);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPut(ApiEndPointConstant.CarbonListing.Update)]
        [ProducesResponseType(typeof(BaseResponse<CarbonListingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonListingResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CarbonListingUpdateRequest request)
        {
            var response = await _carbonListingService.Update(id, request);
            return StatusCode(StatusCodes.Status200OK, response);
        }





    }
}
