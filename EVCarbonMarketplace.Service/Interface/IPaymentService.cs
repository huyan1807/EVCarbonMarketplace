using EVCarbonMarketplace.Model.Payload.Request.Payment;
using EVCarbonMarketplace.Model.Payload.Response;
using EVCarbonMarketplace.Model.Payload.Response.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IPaymentService
    {
        Task<BaseResponse<string>> Create(PaymentRequest request);
        Task<BaseResponse<string>> HandleWebhook(WebhookNotification notification);
    }
}
