using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Request.Payment;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Payment;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EVCarbonMarketplace.API.Controllers
{

    public class PaymentController : BaseController<PaymentController>
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(ILogger<PaymentController> logger ,IPaymentService paymentService) : base(logger)
        {
            _paymentService = paymentService;
        }
        [HttpPost(ApiEndPointConstant.Payment.Create)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Create([FromBody] PaymentRequest request)
        {
            var response = await _paymentService.Create(request);
            return StatusCode(int.Parse(response.Status), response);

        }
        [HttpPost(ApiEndPointConstant.Payment.Webhook)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Webhook([FromBody] WebhookNotification notification)
        {
            var response = await _paymentService.HandleWebhook(notification);
            return StatusCode(int.Parse(response.Status), response);

        }
    }
}
