using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Payload.Request.Account;
using EVCarbonMarketplace.Model.Payload.Request.Owner;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IOwnerService
    {
        Task<BaseResponse<RegisterResponse>> Register(RegisterOwnerRequest request);

    }
}
