using Microsoft.AspNetCore.Identity.UI.Services;

using System.Text;

namespace Juniper.Services;

public class EmailMessage
{
    private readonly string to;
    private readonly string subject;
    private readonly StringBuilder body = new();

    public EmailMessage(string to, string subject)
    {
        this.to = to;
        this.subject = subject;
    }

    public static EmailMessage operator+(EmailMessage message, string text)
    {
        message.body.AppendLine(text);
        return message;
    }

    public static Task operator >>(EmailMessage message, IEmailSender email) =>
        email.SendEmailAsync(message.to, message.subject, message);

    public static implicit operator string(EmailMessage message) =>
        message.ToString();

    public override string ToString() =>
        body.ToString();

    private static string Tag(string tagName, string inner) =>
        $"<{tagName}>{inner}</{tagName}>";

    public static string H1(string inner) =>
        Tag("h1", inner);

    public static string P(string messsage) =>
        Tag("p", messsage);

    public static string Em(string message) =>
        Tag("em", message);

    public static string Thanks(string salutation) =>
        P($"Thank you<br/>- {Em(salutation)}");

    public static string A(string href, string? messsage = null) =>
        $@"<a href=""{href}"">{messsage ?? href}</a>";
}
