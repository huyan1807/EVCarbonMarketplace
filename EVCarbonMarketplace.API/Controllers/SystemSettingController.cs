using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.SystemSetting;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class SystemSettingController : BaseController<SystemSettingController>
    {
        private readonly ISystemSettingService _systemSettingService;
        public SystemSettingController(ILogger<SystemSettingController> logger, ISystemSettingService systemSettingService) : base(logger)
        {
            _systemSettingService = systemSettingService;

        }
            
        [HttpGet(ApiEndPointConstant.SystemSetting.GetAll)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var response = await _systemSettingService.GetAll();
            return StatusCode(int.Parse(response.Status), response);

        }
        [Authorize(Roles = "Admin,Cva")]
        [HttpPut(ApiEndPointConstant.SystemSetting.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] SystemSettingRequest request)
        {
            var response = await _systemSettingService.Update(request);
            return StatusCode(int.Parse(response.Status), response);
        }

    }
}
