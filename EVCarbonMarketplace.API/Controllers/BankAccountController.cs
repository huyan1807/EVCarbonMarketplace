using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.BankAccount;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.BankAccount;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class BankAccountController : BaseController<BankAccountController>
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(ILogger<BankAccountController> logger ,IBankAccountService bankAccountService) : base(logger)
        {
            _bankAccountService = bankAccountService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost(ApiEndPointConstant.BankAccount.Create)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> CreateBankAccount([FromBody] CreateBankAccountRequest request)
        {
            var result = await _bankAccountService.CreateBankAccount(request);
            return StatusCode(int.Parse(result.Status), result);
        }
        [HttpGet(ApiEndPointConstant.BankAccount.GetMyBankAccounts)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetBankAccounts([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var result = await _bankAccountService.GetBankAccounts(page,size);
            return StatusCode(int.Parse(result.Status), result);
        }
        [HttpPut(ApiEndPointConstant.BankAccount.SetDefault)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> SetDefaultBankAccount([FromRoute] Guid id)
        {
            var result = await _bankAccountService.SetDefaultBankAccount(id);
            return StatusCode(int.Parse(result.Status), result);
        }
        [HttpGet(ApiEndPointConstant.BankAccount.GetDefault)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<BankAccountResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetDefaultBankAccount()
        {
            var result = await _bankAccountService.GetDefaultBankAccount();
            return StatusCode(int.Parse(result.Status), result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiEndPointConstant.BankAccount.Delete)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteBankAccount([FromRoute] Guid id)
        {
            var result = await _bankAccountService.DeleteBankAccount(id);
            return StatusCode(int.Parse(result.Status), result);
        }


    }
}
