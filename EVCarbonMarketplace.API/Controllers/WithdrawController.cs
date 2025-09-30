using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.Withdraw;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Withdraw;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class WithdrawController : BaseController<WithdrawController>
    {
        private readonly IWithdrawService _withdrawService;
        public WithdrawController(ILogger<WithdrawController> logger , IWithdrawService withdrawService) : base(logger)
        {
            _withdrawService = withdrawService;
        }
        [HttpPost(ApiEndPointConstant.Withdraw.Create)]
        [ProducesResponseType(typeof(BaseResponse<WithdrawResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<WithdrawResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> RequestWithdraw([FromBody] WithdrawRequest request)
        {
            var result = await _withdrawService.RequestWithdraw(request);
            return StatusCode(int.Parse(result.Status), result);
        }
        [HttpGet(ApiEndPointConstant.Withdraw.GetMyWithdraws)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<WithdrawResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<WithdrawResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetWithdrawHistory([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] WithdrawEnum? status = null)
        {
            var result = await _withdrawService.GetWithdrawHistory(page, size, status);
            return StatusCode(int.Parse(result.Status), result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut(ApiEndPointConstant.Withdraw.UpdateStatus)]
        [ProducesResponseType(typeof(BaseResponse<WithdrawResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<WithdrawResponse>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> UpdateWithdrawStatus([FromForm] UpdateWithdrawRequest request)
        {
            var result = await _withdrawService.UpdateWithdrawStatus(request);
            return StatusCode(int.Parse(result.Status), result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiEndPointConstant.Withdraw.GetAllWithdraws)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<WithdrawResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<WithdrawResponse>>), StatusCodes.Status400BadRequest)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAllWithdraw([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] WithdrawEnum? status = null)
        {
            var result = await _withdrawService.GetAllWithdraw(page, size, status);
            return StatusCode(int.Parse(result.Status), result);
        }

    }
}
