using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Payload.Request.Authentication;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Authentication;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class AuthenticateService : BaseService<AuthenticateService>, IAuthenticateService
    {
        public AuthenticateService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<AuthenticateService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest request)
        {
            Expression<Func<Account, bool>> searchFilter = p =>
                 (p.Username.Equals(request.UsernameOrEmail)
                 || p.Email.Equals(request.UsernameOrEmail)
                 || p.Phone.Equals(request.UsernameOrEmail)) &&
                 p.Password.Equals(PasswordUtil.HashPassword(request.Password)) &&
                 (p.Role == RoleEnum.Admin.GetDescriptionFromEnum() ||
                 p.Role == RoleEnum.CcBuyer.GetDescriptionFromEnum() ||
                 p.Role == RoleEnum.Cva.GetDescriptionFromEnum() ||
                 p.Role == RoleEnum.EvOwner.GetDescriptionFromEnum()) &&
                 p.IsActive == true &&
                 p.DeleteAt == null;
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: searchFilter);

            if (account == null)
            {
                throw new BadHttpRequestException("Tài khoản hoặc mật khẩu không đúng");
            }

          
            RoleEnum role = EnumUtil.ParseEnum<RoleEnum>(account.Role);
            Tuple<string, Guid> guildClaim = new Tuple<string, Guid>("accountId", account.Id);
            var token = JwtUtil.GenerateJwtToken(account, guildClaim);

            var response = new AuthenticateResponse
            {
                AccessToken = token,
                AccountId = account.Id,
                AvatarUrl = account.AvatarUrl,
                Email = account.Email,
                FullName = account.FullName,
                Phone = account.Phone,
                Username = account.Username,
                Role = account.Role
            };

            return new BaseResponse<AuthenticateResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Đăng nhập thành công",
                Data = response
            };
        }

       
    }
}
