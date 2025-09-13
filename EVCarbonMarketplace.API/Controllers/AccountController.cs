using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.Account;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Model.Payload.Response.User;
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
        [HttpPost(ApiEndPointConstant.Account.ChangePassword)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = await _accountService.ChangePassword(request);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpPost(ApiEndPointConstant.Account.ForgotPassword)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var response = await _accountService.ForgotPassword(email);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpPost(ApiEndPointConstant.Account.VerifyOtp)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var response = await _accountService.VerifyOtp(request.Email, request.Otp);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpPost(ApiEndPointConstant.Account.ResetPassword)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var response = await _accountService.ResetPassword(request);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpPost(ApiEndPointConstant.Account.ChangeAvatar)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GetUserResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ChangeAvatar(IFormFile file)
        {
            var response = await _accountService.ChangeAvatar(file);
            return StatusCode(int.Parse(response.Status), response);
        }


    }
}
