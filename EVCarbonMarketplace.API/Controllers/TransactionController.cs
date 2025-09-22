using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Transaction;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace EVCarbonMarketplace.API.Controllers
{

    public class TransactionController : BaseController<TransactionController>
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ILogger<TransactionController> logger , ITransactionService transactionService) : base(logger)
        {
            _transactionService = transactionService;
        }
        [HttpPost(ApiEndPointConstant.Transaction.Purchase)]
        [ProducesResponseType(typeof(BaseResponse<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<TransactionResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Purchase([FromRoute] Guid listingId)
        {
            var response = await _transactionService.Purchase(listingId);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpGet(ApiEndPointConstant.Transaction.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<List<TransactionResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<TransactionResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll([FromQuery] TransactionEnum? type = null, [FromQuery] TransactionStatusEnum? status = null, int page = 1 , int size = 10)
        {
            var response = await _transactionService.GetAll(page,size,type,status);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpGet(ApiEndPointConstant.Transaction.GetMyTransactions)]
        [ProducesResponseType(typeof(BaseResponse<List<TransactionResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<TransactionResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetMyTransaction([FromQuery] TransactionEnum? type = null, [FromQuery] TransactionStatusEnum? status = null, int page = 1, int size = 10)
        {
            var response = await _transactionService.GetMyTransaction(page, size, type, status);
            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}
