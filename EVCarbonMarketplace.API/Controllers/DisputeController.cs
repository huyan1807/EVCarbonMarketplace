using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Enum;
using EVCarbonMarketplace.Model.Payload.Request.Dispute;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class DisputeController : BaseController<DisputeController>
    {
        private readonly IDisputeService _disputeService;
        public DisputeController(ILogger<DisputeController> logger ,IDisputeService disputeService) : base(logger)
        {
            _disputeService = disputeService;
        }
        [HttpGet(ApiEndPointConstant.Dispute.GetDisputeTypes)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDisputeTypes()
        {
            var disputeTypes = _disputeService.GetDisputeTypes();
            return Ok(disputeTypes);
        }
        [HttpPost(ApiEndPointConstant.Dispute.Create)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] DisputeRequest request)
        {
            var response = await _disputeService.Create(request);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpPut(ApiEndPointConstant.Dispute.UpdateStatus)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromForm] UpdateDisputeStatusRequest request)
        {
            var response = await _disputeService.Update(request);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Dispute.GetMyDisputes)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMyDisputes([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] DisputeTypeEnum? type = null, [FromQuery] DisputeStatusEnum? status = null)
        {
            var response = await _disputeService.GetMyDisputes(page, size,type,status);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Dispute.GetById)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var response = await _disputeService.GetById(id);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Dispute.GetAll)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] DisputeTypeEnum? type = null, [FromQuery] DisputeStatusEnum? status = null)
        {
            var response = await _disputeService.GetAll(page, size, type, status);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpDelete(ApiEndPointConstant.Dispute.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _disputeService.Delete(id);
            return StatusCode(int.Parse(response.Status), response);
        }
    }
}
