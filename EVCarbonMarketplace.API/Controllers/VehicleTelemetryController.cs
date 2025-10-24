using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleTelemetry;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class VehicleTelemetryController : BaseController<VehicleTelemetryController>
    {
        private readonly IVehicleTelemetryService _vehicleTelemetryService;
        public VehicleTelemetryController(ILogger<VehicleTelemetryController> logger , IVehicleTelemetryService vehicleTelemetryService) : base(logger)
        {
            _vehicleTelemetryService = vehicleTelemetryService;
        }
        [HttpGet(ApiEndPointConstant.VehicleTelemetry.GetByEVehicle)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<VehicleTelemetryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<VehicleTelemetryResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetByEVehicle([FromRoute] Guid id, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _vehicleTelemetryService.GetVehicleTelemetry(page, size, id);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [HttpDelete(ApiEndPointConstant.VehicleTelemetry.Delete)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _vehicleTelemetryService.Delete(id);           
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
