using AutoMapper;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Request.Cva;
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
    public class CvaService : BaseService<CvaService>, ICvaService
    {
        private readonly IUploadService _uploadService;
        public CvaService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<CvaService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadService uploadService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _uploadService = uploadService;
        }

        public async Task<BaseResponse<RegisterResponse>> Register(RegisterCvaRequest request)
        {

            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync();
            if (accounts.Any(a => a.Username.Equals(request.UserName)))
                throw new BadHttpRequestException("Tên đăng nhập đã tồn tại");
            if (accounts.Any(a => a.Email.Equals(request.Email)))
                throw new BadHttpRequestException("Email đã tồn tại");
            if (accounts.Any(a => a.Phone.Equals(request.Phone)))
                throw new BadHttpRequestException("Số điện thoại đã tồn tại");
         
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
