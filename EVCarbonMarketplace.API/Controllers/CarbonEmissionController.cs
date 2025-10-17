using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Response.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EVCarbonMarketplace.Model.Entity;
using EVCarbonMarketplace.Model.Payload.Response.CarbonEmission;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Enum;
using Microsoft.AspNetCore.Authorization;

namespace EVCarbonMarketplace.API.Controllers
{

    public class CarbonEmissionController : BaseController<CarbonEmissionController>
    {
        private readonly ICarbonEmissionService _carbonEmissionService;
        public CarbonEmissionController(ILogger<CarbonEmissionController> logger ,ICarbonEmissionService carbonEmissionService) : base(logger)
        {
            _carbonEmissionService = carbonEmissionService;
        }

        [HttpPost(ApiEndPointConstant.CarbonEmissions.Create)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmission>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmissionResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ImportTelemetryFromFile([FromRoute] Guid Id, IFormFile file)
        {
            var response = await _carbonEmissionService.ImportTelemetryFromFileAsync(Id, file);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles = "Admin,Cva")]
        [HttpGet(ApiEndPointConstant.CarbonEmissions.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<CarbonEmissionManageResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmissionManageResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll([FromQuery] CarbonEmissionEnum? status = null,[FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _carbonEmissionService.GetAll(page, size, status);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet(ApiEndPointConstant.CarbonEmissions.GetById)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmissionDetailResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmissionDetailResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _carbonEmissionService.GetById(id);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles = "Admin,Cva")]
        [HttpPut(ApiEndPointConstant.CarbonEmissions.ApproveEmission)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmissionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<CarbonEmissionResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ApproveEmission([FromRoute] Guid id, [FromQuery] CarbonEmissionEnum status)
        {
            var response = await _carbonEmissionService.ApproveEmission(id, status);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [Authorize(Roles = "Admin,Cva")]
        [HttpDelete(ApiEndPointConstant.CarbonEmissions.Delete)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteEmission([FromRoute] Guid id)
        {
            var response = await _carbonEmissionService.DeleteEmission(id);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        }
}
