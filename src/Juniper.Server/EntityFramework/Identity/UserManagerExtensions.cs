using Juniper.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

using System.Globalization;
using System.Text;

using static Juniper.Services.EmailMessage;

namespace Juniper.EntityFramework.Identity;

public static class UserManagerExtensions
{
    public static async Task<IdentityResult> SendPasswordChangeEmailAsync(
        this UserManager<IdentityUser> users,
        HttpRequest request,
        ILogger logger,
        IUrlHelper Url,
        IEmailSender email,
        IdentityUser user,
        string adminEmail,
        string siteName,
        string salutation,
        bool send)
    {
        try
        {
            if (user?.Email is null)
            {
                throw new ArgumentNullException("user.Email");
            }

            var hasPassword = !string.IsNullOrEmpty(user.PasswordHash);
            var code = await users.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var expiry = DateTime.UtcNow
                .AddDays(1)
                .ToLocalTime()
                .ToString(CultureInfo.CurrentCulture);

            var callbackUrl = Url.Page("/Account/ResetPassword", new { area = "Identity", code, email = user.Email });
            if (callbackUrl is null)
            {
                throw new ArgumentNullException(nameof(callbackUrl));
            }

            var userName = user.UserName ?? user.Email;


            var siteUrl = $"{request.Scheme}://{request.Host}";
            var userProfileUrl = $"{siteUrl}/Editor/Users/Detail/{user.Id}";
            callbackUrl = $"{siteUrl}{callbackUrl}";

            siteUrl = A(siteUrl);
            userProfileUrl = A(userProfileUrl, userName);
            callbackUrl = A(callbackUrl);

            var subjectToUser = hasPassword ? $"Reset Your {siteName} Password" : $"Welcome to {siteName}";
            var subjectToAdmin = hasPassword ? $"Reseting {siteName} Password for {user.UserName}" : $"Requesting new {siteName} password for {user.UserName}";
            var title = hasPassword ? "Reset" : "Set";
            var toUser = new EmailMessage(user.Email, subjectToUser);
            toUser += H1($"{title} Your {siteName} Password");

            var toAdmin = new EmailMessage(adminEmail, subjectToAdmin);
            toAdmin += H1($"User {user.UserName} must {title} their {siteName} Password");

            if (hasPassword)
            {
                toUser += P($@"We received a request from you to reset your {siteUrl} password.
Please use the following link to visit a secure Web page where you may set a new password.
This link will expire on {expiry}.");

                toAdmin += P($@"We sent a request to {userProfileUrl} to reset their {siteUrl} password.
They have until {expiry} to do so.");

            }
            else
            {
                toUser += P($@"We have created a new account for you at {siteUrl}.
Please use the following link to visit a secure Web page where you may set a password for your account.
This link will expire on {expiry}.");
                toAdmin += P($@"We created a new account for {userProfileUrl} at {siteUrl}.
They have until {expiry} to set their password.");
            }

            toUser += P(callbackUrl);

            if (hasPassword)
            {
                toUser += P("If you did not request a password reset, you may ignore this email.");
            }

            toUser += Thanks(salutation);
            toAdmin += Thanks(salutation);

            if (send)
            {
                await (toUser >> email);
                await (toAdmin >> email);
            }
            else
            {
                logger.LogInformation("Message sent to user: {toUser}", toUser);
                logger.LogInformation("Message sent to admin: {toAdmin}", toAdmin);
            }

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
