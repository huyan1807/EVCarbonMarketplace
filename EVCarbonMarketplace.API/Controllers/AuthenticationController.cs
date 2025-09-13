using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.Authentication;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Authentication;
using EVCarbonMarketplace.Service.Interface;
using Google.Apis.Oauth2.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class AuthenticationController : BaseController<AuthenticationController>
    {
        private readonly IAuthenticateService _authenticateService;
        public AuthenticationController(ILogger<AuthenticationController> logger ,IAuthenticateService authenticateService) : base(logger)
        {

            _authenticateService = authenticateService;
        }

        [HttpPost(ApiEndPointConstant.Authentication.Authenticate)]
        [ProducesResponseType(typeof(BaseResponse<AuthenticateResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthenticateResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            var response = await _authenticateService.Authenticate(request);
            return StatusCode(int.Parse(response.Status), response);
        }

    }
}
