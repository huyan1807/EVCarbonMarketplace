using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.Owner;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class OwnerController : BaseController<OwnerController>
    {
        private readonly IOwnerService _ownerService;
        public OwnerController(ILogger<OwnerController> logger ,IOwnerService ownerService) : base(logger)
        {
            _ownerService = ownerService;
        }
        [HttpPost(ApiEndPointConstant.Owner.Register)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromForm] RegisterOwnerRequest request)
        {
            var result = await _ownerService.Register(request);
            return StatusCode(int.Parse(result.Status), result);
        }
    }
}
