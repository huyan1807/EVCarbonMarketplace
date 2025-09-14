using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Response.GoogleAuthentication;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class GoogleAuthenticationService : BaseService<GoogleAuthenticationService>, IGoogleAuthenticationService
    {
        public GoogleAuthenticationService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<GoogleAuthenticationService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async  Task<GoogleAuthResponse> GoogleAuthenticate(HttpContext context)
        {
            var authenticateResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult.Principal == null) return null;
            var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var avatar = authenticateResult.Principal.FindFirstValue("picture");
            if (email == null) return null;
            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");

            return new GoogleAuthResponse
            {
                FullName = name,
                Email = email,
                Token = accessToken,
                Avatar = avatar
            };
        }
    }
}
