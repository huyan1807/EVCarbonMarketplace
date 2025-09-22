using EVCarbonMarketplace.Model.Payload.Response.Authentication;
using EVCarbonMarketplace.Model.Payload.Response.GoogleAuthentication;
using EVCarbonMarketplace.Model.Payload.Response.User;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.User;
using EVCarbonMarketplace.Model.Enum;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IUserService
    {
        Task<bool> GetAccountByEmail(string email);

        Task<BaseResponse<GetUserResponse>> CreateNewUserAccountByGoogle(GoogleAuthResponse googleAuthResponse);

        Task<BaseResponse<AuthenticateResponse>> CreateTokenByEmail(string email);
        Task<BaseResponse<IPaginate<GetUserResponse>>> GetAllUsers(int page, int size , RoleEnum? role);

        Task<BaseResponse<GetUserResponse>> GetUserProfile();

        Task<BaseResponse<GetUserResponse>> GetUser(Guid id);

        Task<BaseResponse<bool>> DeleteUser(Guid id);

        Task<BaseResponse<GetUserResponse>> UpdateUser(UpdateUserRequest request);

    }
}
