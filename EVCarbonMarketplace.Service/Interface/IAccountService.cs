using EVCarbonMarketplace.Model.Payload.Request.Account;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Model.Payload.Response.User;
using Microsoft.AspNetCore.Http;
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
        Task<BaseResponse<bool>> ChangePassword(ChangePasswordRequest request);

        Task<BaseResponse<bool>> ForgotPassword(string email);

        Task<BaseResponse<GetUserResponse>> VerifyOtp(string email, string otp);

        Task<BaseResponse<GetUserResponse>> ResetPassword(ResetPasswordRequest request);

        Task<BaseResponse<GetUserResponse>> ChangeAvatar(IFormFile file);
    }
}
