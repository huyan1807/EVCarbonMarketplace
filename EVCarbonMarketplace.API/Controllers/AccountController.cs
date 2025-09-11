using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{
  
    public class AccountController : BaseController<AccountController>
    {
        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger ,IAccountService accountService) : base(logger)
        {
            _accountService = accountService;
        }
        [HttpPost(ApiEndPointConstant.Account.Otp)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            var response = await _accountService.SendOtp(email);
            return StatusCode(int.Parse(response.Status), response);
        }

        [HttpPost(ApiEndPointConstant.Account.Register)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<RegisterResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            var response = await _accountService.Register(request);
            return StatusCode(int.Parse(response.Status), response);
        }

    }
}
