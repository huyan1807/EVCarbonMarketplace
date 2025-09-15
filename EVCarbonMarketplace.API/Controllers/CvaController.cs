using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.Cva;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class CvaController : BaseController<CvaController>
    {
        private readonly ICvaService _cvaService;
        public CvaController(ILogger<CvaController> logger ,ICvaService cvaService) : base(logger)
        {
            _cvaService = cvaService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost(ApiEndPointConstant.Cva.Register)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromForm] RegisterCvaRequest request)
        {
            var result = await _cvaService.Register(request);
            return StatusCode(int.Parse(result.Status), result);
        }
    }
}
