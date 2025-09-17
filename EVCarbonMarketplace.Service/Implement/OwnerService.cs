using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Request.Owner;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Account;
using EVCarbonMarketplace.Model.Utils;
using EVCarbonMarketplace.Repository.Interface;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class OwnerService : BaseService<OwnerService>, IOwnerService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IUploadService _uploadService;
        public OwnerService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<OwnerService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor , IUploadService uploadService, IConnectionMultiplexer redis) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
            _redis = redis;
        }

        public async Task<BaseResponse<RegisterResponse>> Register(RegisterOwnerRequest request)
        {
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();
            if (accounts.Any(a => a.Username.Equals(request.UserName)))
                throw new BadHttpRequestException("Tên đăng nhập đã tồn tại");
            if (accounts.Any(a => a.Email.Equals(request.Email)))
                throw new BadHttpRequestException("Email đã tồn tại");
            if (accounts.Any(a => a.Phone.Equals(request.Phone)))
                throw new BadHttpRequestException("Số điện thoại đã tồn tại");
            var redisDb = _redis.GetDatabase();
            if (redisDb == null) throw new RedisServerException("Không thể kết nối tới Redis");

            var key = "emailOtp:" + request.Email;
            var stored = await redisDb.StringGetAsync(key);
            if (string.IsNullOrEmpty(stored))
                throw new NotFoundException("Không tìm thấy mã OTP");
            if (!stored.Equals(request.Otp))
                throw new BadHttpRequestException("Mã OTP không chính xác");

            var account = _mapper.Map<Account>(request);
            account.AvatarUrl = await _uploadService.UploadImage(request.AvatarUrl);

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            var wallet = new Wallet
            {

                Id = Guid.NewGuid(),
                AccountId = account.Id,
                CarbonUnit = 0,
                Cash = 0,
                IsActive = true,
                CreateAt = TimeUtil.GetCurrentSEATime(),
                UpdateAt = TimeUtil.GetCurrentSEATime()
            };
            await _unitOfWork.GetRepository<Wallet>().InsertAsync(wallet);


            var isSuccess = await _unitOfWork.CommitAsync() > 0;
            await redisDb.KeyDeleteAsync(key);

            if (!isSuccess) throw new Exception("Có lỗi trong quá trình đăng kí tài khoản");

            return new BaseResponse<RegisterResponse>
            {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Tạo tài khoản thành công",
                Data = _mapper.Map<RegisterResponse>(account)
            };
        }
    }
}
