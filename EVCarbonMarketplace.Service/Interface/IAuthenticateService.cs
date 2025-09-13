using EVCarbonMarketplace.Model.Payload.Request.Authentication;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IAuthenticateService
    {
        Task<BaseResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest request);

    }
}
