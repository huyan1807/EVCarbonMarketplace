using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class AnalyticsController : BaseController<AnalyticsController>
    {
        private readonly IAnalyticsService _analyticsService;
        public AnalyticsController(ILogger<AnalyticsController> logger , IAnalyticsService analyticsService) : base(logger)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet(ApiEndPointConstant.Analytics.GetUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _analyticsService.GetUsers();
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Analytics.GetRealtimeUsers)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRealtimeUsers()
        {
            var response = await _analyticsService.GetRealtimeUsers();
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Analytics.GetRegisteredUsersByDay)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRegisteredUsersByDay()
        {
            var response = await _analyticsService.GetRegisteredUsersByDay();
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Analytics.GetFinanceStats)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFinanceStats()
        {
            var response = await _analyticsService.GetFinanceStats();
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Analytics.GetTransactionStats)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionStats()
        {
            var response = await _analyticsService.GetTransactionStats();
            return StatusCode(int.Parse(response.Status), response);
        }



    }
}
