using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IAccountService
    {
        Task<BaseResponse<RegisterResponse>> Register(RegisterRequest request);
        Task<BaseResponse<bool>> SendOtp(string email);
    }
}
