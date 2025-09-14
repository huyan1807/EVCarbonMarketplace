using EVCarbonMarketplace.Model.Payload.Response.GoogleAuthentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IGoogleAuthenticationService
    {
        Task<GoogleAuthResponse> GoogleAuthenticate(HttpContext context);
    }
}
