using EVCarbonMarketplace.Model.Payload.Request.Notification;
using EVCarbonMarketplace.Model.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface INotificationService
    {
        Task<BaseResponse<NotificationRequest>> Create(NotificationRequest request);
        Task<BaseResponse<bool>> DeleteNotification(string id);
        Task<BaseResponse<bool>> MarkRead(string id);
        Task<BaseResponse<bool>> MarkAllRead();

    }
}
