using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Wallet;
using EVCarbonMarketplace.Service.Implement;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class WalletController : BaseController<WalletController>
    {
        private readonly IWalletService _walletService;

        public WalletController(ILogger<WalletController> logger ,IWalletService walletService) : base(logger)
        {
            _walletService = walletService;

        }

        [HttpGet(ApiEndPointConstant.Wallet.GetMyWallet)]
        [ProducesResponseType(typeof(BaseResponse<WalletResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<WalletResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetMyWallet()
        {
            var response = await _walletService.GetMyWallet();
            return StatusCode(int.Parse(response.Status), response);
        }
        

    }
}
