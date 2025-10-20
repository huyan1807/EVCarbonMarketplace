using EVCarbonMarketplace.Model.Payload.Request.Notification;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EVCarbonMarketplace.API.Constant;
using EVCarbonMarketplace.Model.Payload.Response.Wallet;

namespace EVCarbonMarketplace.API.Controllers
{

    public class NotificationController : BaseController<NotificationController>
    {
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger , INotificationService notificationService) : base(logger)
        {
            _notificationService = notificationService;
        }



        [HttpDelete(ApiEndPointConstant.Notification.Delete)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Delete(string id)
        {   
            var res = await _notificationService.DeleteNotification(id);
            return StatusCode(int.Parse(res.Status), res);
        }
        [HttpPost(ApiEndPointConstant.Notification.MarkRead)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> MarkRead(string id)
        {
            var res = await _notificationService.MarkRead(id);
            return StatusCode(int.Parse(res.Status), res);
        }

        [HttpPost(ApiEndPointConstant.Notification.MarkAllRead)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> MarkAllRead()
        {
            var res = await _notificationService.MarkAllRead();
            return StatusCode(int.Parse(res.Status), res);
        }

    }
}
