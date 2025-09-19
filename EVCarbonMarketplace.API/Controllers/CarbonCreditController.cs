using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.CarbonCredit;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class CarbonCreditController : BaseController<CarbonCreditController>
    {
        private readonly ICarbonCreditService _carbonCreditService;
        public CarbonCreditController(ILogger<CarbonCreditController> logger , ICarbonCreditService carbonCreditService) : base(logger)
        {
            _carbonCreditService = carbonCreditService;
        }
        [HttpGet(ApiEndPointConstant.CarbonCredits.GetMyCredits)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonCreditResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonCreditResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetMyCredits([FromQuery] CarbonCreditEnum status)
        {
            var response = await _carbonCreditService.GetMyCredits(status);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles = "Cva,Admin")]
        [HttpGet(ApiEndPointConstant.CarbonCredits.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonCreditManageResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonCreditManageResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll([FromQuery] CarbonCreditEnum status, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _carbonCreditService.GetAllCredits(page, size,status);
            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}
