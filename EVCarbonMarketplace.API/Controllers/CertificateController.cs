using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class CertificateController : BaseController<CertificateController>
    {
        private readonly ICertificateService _certificateService;
        public CertificateController(ILogger<CertificateController> logger , ICertificateService certificateService) : base(logger)
        {
            _certificateService = certificateService;
        }
        [HttpPost(ApiEndPointConstant.Certificate.Generate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestCertificate([FromRoute] Guid carbonCreditId)
        {
            var response = await _certificateService.RequestCertificate(carbonCreditId);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Certificate.GetMyCertificates)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMyCertificates([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _certificateService.GetMyCertificate(page, size);
            return StatusCode(int.Parse(response.Status), response);
        }
        [HttpGet(ApiEndPointConstant.Certificate.GetCertificate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCertificate([FromRoute] Guid carbonCreditId)
        {
            var response = await _certificateService.GetCertificate(carbonCreditId);
            return StatusCode(int.Parse(response.Status), response);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet(ApiEndPointConstant.Certificate.GetAll)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _certificateService.GetAll(page, size);
            return StatusCode(int.Parse(response.Status), response);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete(ApiEndPointConstant.Certificate.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await _certificateService.Delete(id);
            return StatusCode(int.Parse(response.Status), response);
        }
    }
}
