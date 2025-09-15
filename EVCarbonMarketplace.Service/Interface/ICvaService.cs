using EVCarbonMarketplace.Model.Payload.Request.Cva;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface ICvaService
    {
        Task<BaseResponse<RegisterResponse>> Register (RegisterCvaRequest request);
    }
}
