using EVCarbonMarketplace.Model.Payload.Response.Authentication;
using EVCarbonMarketplace.Model.Payload.Response.GoogleAuthentication;
using EVCarbonMarketplace.Model.Payload.Response.User;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IUserService
    {
        Task<bool> GetAccountByEmail(string email);

        Task<BaseResponse<GetUserResponse>> CreateNewUserAccountByGoogle(GoogleAuthResponse googleAuthResponse);

        Task<BaseResponse<AuthenticateResponse>> CreateTokenByEmail(string email);
    }
}
