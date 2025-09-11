using EVCarbonMarketplace.Model.Payload.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Interface
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage emailMessage);
    }
}
