using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class SystemSettingController : BaseController<SystemSettingController>
    {
        private readonly ISystemSettingService _systemSettingService;
        public SystemSettingController(ILogger<SystemSettingController> logger ,ISystemSettingService systemSettingService) : base(logger)
        {
            _systemSettingService = systemSettingService;

        }
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiEndPointConstant.SystemSetting.GetTransactionFee)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionFee()
        {
            var response = await _systemSettingService.GetTransactionFee();
            return StatusCode(int.Parse(response.Status), response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut(ApiEndPointConstant.SystemSetting.UpdateTransactionFee)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTransactionFee([FromQuery] decimal newFeeRate)
        {
            var response = await _systemSettingService.UpdateTransactionFee(newFeeRate);
            return StatusCode(int.Parse(response.Status), response);
        }
    }
}
