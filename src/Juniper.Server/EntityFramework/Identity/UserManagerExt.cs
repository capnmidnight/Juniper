using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using System.Globalization;
using System.Text;

namespace Juniper.EntityFramework.Identity
{
    public static class UserManagerExt
    {
        public static async Task<IdentityResult> SendPasswordChangeEmail(
            this UserManager<IdentityUser> users,
            HttpRequest request,
            IUrlHelper urls,
            IEmailSender email,
            IdentityUser user,
            string siteName,
            string siteUrl,
            string salutation)
        {
            try
            {
                var hasPassword = !string.IsNullOrEmpty(user.PasswordHash);
                var code = await users.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var expiry = DateTime.Now.AddDays(1).ToString(CultureInfo.CurrentCulture);
                var callbackUrl = urls.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, email = user.Email },
                    protocol: request.Scheme);
                var subject = hasPassword ? $"Reset Your {siteName} Password" : $"Welcome to {siteName}";
                var title = hasPassword ? "Reset" : "Set";
                var message = new StringBuilder($"<h1>{title} Your {siteName} Password</h1>\n");
                if (hasPassword)
                {
                    message.AppendLine($@"<p>
    We received a request from you to reset your <a href=""{siteUrl}"">{siteUrl}</a> password.
    Please use the following link to visit a secure Web page where you may set a new password.
    This link will expire on {expiry}.
</p>");

                }
                else
                {
                    message.AppendLine($@"<p>
    We have created a new account for you at <a href=""https://{siteUrl}"">{siteUrl}</a>.
    Please use the following link to visit a secure Web page where you may set a password for your account.
    This link will expire on {expiry}.
</p>");
                }

                message.AppendLine($@"<p>
    <a href=""{callbackUrl}"">{callbackUrl}</a>
</p>");

                if (hasPassword)
                {
                    message.AppendLine("<p>If you did not request a password reset, you may ignore this email.</p>");
                }

                message.AppendLine($"<p>Thank you<br/>- <em>{salutation}</em></p>");

                await email.SendEmailAsync(user.Email, subject, message.ToString());

                return IdentityResult.Success;
            }
            catch (Exception exp)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = exp.GetType().Name,
                    Description = exp.Message
                });
            }
        }
    }
}
