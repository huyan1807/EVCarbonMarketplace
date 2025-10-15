using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EVCarbonMarketplace.API.Controllers
{

    public class GoogleAuthenticationController : BaseController<GoogleAuthenticationController>
    {
        private readonly IGoogleAuthenticationService _googleAuthenticationService;
        private readonly IUserService _userService;
        public GoogleAuthenticationController(ILogger<GoogleAuthenticationController> logger , IGoogleAuthenticationService googleAuthenticationService, IUserService userService) : base(logger)
        {
            _googleAuthenticationService = googleAuthenticationService;
            _userService = userService;
        }
        [HttpGet(ApiEndPointConstant.GoogleAuthentication.GoogleAuthLogin)]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties() { RedirectUri = $"/api/v1/google-auth/sign-in" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }


        [HttpGet(ApiEndPointConstant.GoogleAuthentication.GoogleAuthSignIn)]
        public async Task<IActionResult> SignInGoogle()
        {
            var googleAuthResponse = await _googleAuthenticationService.GoogleAuthenticate(HttpContext);
            var checkAccount = await _userService.GetAccountByEmail(googleAuthResponse.Email);
            if (!checkAccount)
            {
                var response = await _userService.CreateNewUserAccountByGoogle(googleAuthResponse);
                if (response == null)
                {
                    return Problem("Tài khoản không tồn tại");
                }
            }
            var token = await _userService.CreateTokenByEmail(googleAuthResponse.Email);

            var jsonData = JsonConvert.SerializeObject(token);
            string htmlResponse = $@"
            <html>
              <body>
                <script>
                  window.opener.postMessage({jsonData}, '*');
                  window.close();
                </script>
              </body>
            </html>";
            return Content(htmlResponse, "text/html");
            //return StatusCode(int.Parse(token.Status), token.Data);
        }
    }
}
