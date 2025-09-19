using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IWalletService
    {
        Task<BaseResponse<WalletResponse>> GetMyWallet();
    }
}
