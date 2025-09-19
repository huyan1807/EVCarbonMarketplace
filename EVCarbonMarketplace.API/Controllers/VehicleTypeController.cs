using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.VehicleType;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class VehicleTypeController : BaseController<VehicleTypeController>
    {
        private readonly IVehicleTypeService _vehicleTypeService;
        public VehicleTypeController(ILogger<VehicleTypeController> logger ,IVehicleTypeService vehicleTypeService) : base(logger)
        {
            _vehicleTypeService = vehicleTypeService;
        }

        [HttpGet(ApiEndPointConstant.VehicleType.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<VehicleTypeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<VehicleTypeResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll(int page = 1, int size = 10)
        {
            var response = await _vehicleTypeService.GetAll(page, size);
            return StatusCode(int.Parse(response.Status), response);
        }


    }
}
