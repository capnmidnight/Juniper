using Microsoft.AspNetCore.Identity.UI.Services;

using System.Net;
using System.Net.Mail;
using System.Text;

namespace Juniper.Services
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly SmtpClient mailClient;
        private readonly MailAddress from;
        private bool disposedValue;

        public EmailSender(IConfiguration config)
            : this("dlsdc-com.mail.protection.outlook.com", //config.GetValue<string>("Mail:Host"),
                  25, //config.GetValue<int>("Mail:Port"),
                  config.GetValue<string>("Mail:From"),
                  config.GetValue<string>("Mail:User"),
                  config.GetValue<string>("Mail:Password"))
        {
        }

        public EmailSender(string host, int port, string fromUser, string authUser, string password)
        {
            mailClient = new SmtpClient
            {
                Host = host,
                Port = port,
                Credentials = new NetworkCredential(
                    authUser,
                    password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            from = new MailAddress(fromUser);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var msg = new MailMessage(from, new MailAddress(email))
            {
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            await mailClient.SendMailAsync(msg);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mailClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
