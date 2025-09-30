using CloudinaryDotNet.Actions;
using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Paginate;
using EVCarbonMarketplace.Model.Payload.Request.ElectricVehicle;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.ElectricVehicle;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class ElectricVehicleController : BaseController<ElectricVehicleController>
    {
        private readonly IElectricVehicleService _electricVehicleService;
        public ElectricVehicleController(ILogger<ElectricVehicleController> logger ,IElectricVehicleService electricVehicleService) : base(logger)
        {
            _electricVehicleService = electricVehicleService;

        }
        [Authorize(Roles = "EvOwner")]
        [HttpPost(ApiEndPointConstant.EVehicle.ElectricVehicle)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Create([FromForm] ElectricVehicleRequest request)
        {
            var response = await _electricVehicleService.Create(request);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [Authorize(Roles = "EvOwner")]
        [HttpPut(ApiEndPointConstant.EVehicle.Update)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ElectricVehicleUpdateRequest request)
        {
            var response = await _electricVehicleService.Update(id, request);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [Authorize(Roles = "EvOwner")]
        [HttpPut(ApiEndPointConstant.EVehicle.ChangeImage)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> ChangeImage([FromRoute] Guid id, IFormFile file)
        {
            var response = await _electricVehicleService.ChangeImage(id, file);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [Authorize(Roles = "EvOwner")]
        [HttpDelete(ApiEndPointConstant.EVehicle.Delete)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _electricVehicleService.Delete(id);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles = "Admin,Cva")]
        [HttpGet(ApiEndPointConstant.EVehicle.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<ElectricVehicleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<ElectricVehicleResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _electricVehicleService.GetAll(page, size);
            return StatusCode(StatusCodes.Status200OK, response);
        }
        [Authorize(Roles = "EvOwner")]
        [HttpGet(ApiEndPointConstant.EVehicle.GetMyEVehicles)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<ElectricVehicleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IPaginate<ElectricVehicleResponse>>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetMyEVehicles([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _electricVehicleService.GetMyEVehicles(page, size);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet(ApiEndPointConstant.EVehicle.GetById)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<ElectricVehicleResponse>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _electricVehicleService.GetById(id);
            return StatusCode(StatusCodes.Status200OK, response);
        }





    }
}
