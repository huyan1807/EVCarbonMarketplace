using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Authentication;
using EVCarbonMarketplace.Model.Payload.Response.GoogleAuthentication;
using EVCarbonMarketplace.Model.Payload.Response.User;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class UserService : BaseService<UserService>, IUserService
    {
        public UserService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<GetUserResponse>> CreateNewUserAccountByGoogle(GoogleAuthResponse googleAuthResponse)
        {
            var existingUser = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
               predicate: u => u.Email.Equals(googleAuthResponse.Email) &&
                                                       u.IsActive == true);

            if (existingUser != null)
            {
                return new BaseResponse<GetUserResponse>
                {
                    Status = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Tài khoản người dùng đã tồn tại.",
                    Data = new GetUserResponse()
                    {
                        AccountId = existingUser.Id,
                        Email = existingUser.Email,
                        FullName = existingUser.FullName,
                        Phone = existingUser.Phone,
                        AvatarUrl = existingUser.AvatarUrl,
                        DateOfBirth = existingUser.DateOfBirth,
                        Gender = existingUser.Gender,
                      
                    }
                };
            }

            var account = new Account()
            {
                Id = Guid.NewGuid(),
                Email = googleAuthResponse.Email,
                FullName = googleAuthResponse.FullName,
                Username = googleAuthResponse.Email.Split("@")[0],
                Role = RoleEnum.CcBuyer.GetDescriptionFromEnum(),
                IsActive = true,
                Password = PasswordUtil.HashPassword("12345678"),
                Phone = "0000000000",
                DateOfBirth = DateOnly.FromDateTime(TimeUtil.GetCurrentSEATime()),
                Gender = GenderEnum.Male.GetDescriptionFromEnum(),
                AvatarUrl = googleAuthResponse.Avatar,
                CreateAt = TimeUtil.GetCurrentSEATime(),
                UpdateAt = TimeUtil.GetCurrentSEATime(),
            };

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);

          // thiếu wallet

            var isSuccess = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccess)
            {
                throw new Exception("Một lỗi đã xảy ra trong quá trình đăng nhập google");
            }

            return new BaseResponse<GetUserResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo tài khoản thành công",
                Data = new GetUserResponse()
                {
                    AccountId = account.Id,
                    Email = account.Email,
                    FullName = account.FullName,
                    Phone = account.Phone,
                    AvatarUrl = account.AvatarUrl,
                    DateOfBirth = account.DateOfBirth,
                    Gender = account.Gender,
                 
                }
            };
        }

        public async Task<BaseResponse<AuthenticateResponse>> CreateTokenByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Tên người dùng không thể rỗng hoặc để trống", nameof(email));
            }
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: p => p.Email.Equals(email) && p.IsActive == true && p.DeleteAt == null
            );
            if (account == null) throw new NotFoundException("Tài khoản không tìm thấy hoặc đã bị cấm.");
            Tuple<string, Guid> guildClaim = new Tuple<string, Guid>("accountId", account.Id);
            var token = JwtUtil.GenerateJwtToken(account, guildClaim);

            var response = new AuthenticateResponse()
            {
                AccountId = account.Id,
                Email = account.Email,
                Username = account.Username,
                Phone = account.Phone,
                Role = account.Role,
                FullName = account.FullName,
                AccessToken = token,
                AvatarUrl = account.AvatarUrl,
            };

            return new BaseResponse<AuthenticateResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đăng nhập thành công",
                Data = response
            };
        }

        public async Task<bool> GetAccountByEmail(string email)
        {
            if (email == null) throw new BadHttpRequestException("Email không được để null");

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: p => p.Email.Equals(email)
            );
            return account != null;
        }
    }
}
