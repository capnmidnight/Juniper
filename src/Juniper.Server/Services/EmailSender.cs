using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

using System.Net;
using System.Net.Mail;
using System.Text;

namespace Juniper.Services;

public class EmailSender : IEmailSender, IDisposable
{
    private readonly SmtpClient mailClient;
    private readonly MailAddress from;
    private bool disposedValue;

    public EmailSender(IConfiguration config)
        : this(config.GetValue<string>("Mail:Host") ?? throw new Exception("No Mail:Host"),
              config.GetValue<int?>("Mail:Port") ?? throw new Exception("No Mail:Port"),
              config.GetValue<string>("Mail:From") ?? throw new Exception("No Mail:From"),
              config.GetValue<string>("Mail:User") ?? throw new Exception("No Mail:User"),
              config.GetValue<string>("Mail:Password") ?? throw new Exception("No Mail:Password"))
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
