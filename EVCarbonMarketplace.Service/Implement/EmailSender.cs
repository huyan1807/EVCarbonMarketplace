using EVCarbonMarketplace.Model.Payload.Settings;
using EVCarbonMarketplace.Service.Interface;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Service.Implement
{
    public class EmailSender : IEmailSender
    {
        private readonly SMTPSettings _smtpSettings;
        public EmailSender(IOptions<SMTPSettings> settings)
        {
            _smtpSettings = settings.Value;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            email.To.Add(new MailboxAddress(emailMessage.ToAddress.Split('@')[0], emailMessage.ToAddress));
            email.Subject = emailMessage.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = emailMessage.Body
            };

            email.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(email);
            }
            catch
            {
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
