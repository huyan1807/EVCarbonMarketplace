using AutoMapper;
using Microsoft.EntityFrameworkCore;

using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Response;
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
using EVCarbonMarketplace.Model.Exceptions;
using EVCarbonMarketplace.Model.Payload.Response.Wallet;

namespace EVCarbonMarketplace.Service.Implement
{
    public class WalletService : BaseService<WalletService>, IWalletService
    {
        public WalletService(IUnitOfWork<EvcarbonMarketplaceContext> unitOfWork, ILogger<WalletService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<BaseResponse<WalletResponse>> GetMyWallet()
        {
            var accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: x => x.Id == accountId && x.IsActive == true && x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy tài khoản");
            var wallet = await _unitOfWork.GetRepository<Wallet>().SingleOrDefaultAsync(
                predicate: x => x.AccountId == account.Id && x.IsActive == true && x.DeleteAt == null
            ) ?? throw new NotFoundException("Không tìm thấy ví");

            return new BaseResponse<WalletResponse> {
                Status = StatusCodes.Status200OK.ToString(),
                Message = "Lấy ví thành công",
                Data = new WalletResponse
                {
                    Id = wallet.Id,
                    CarbonUnit = wallet.CarbonUnit ?? 0,
                    Cash = wallet.Cash ?? 0
                }
            };

        }
    }
}
