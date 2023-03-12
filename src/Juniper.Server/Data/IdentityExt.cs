using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using System.Text;

namespace Juniper.Data
{
    public record BootsrapUser(string Email, bool Bootstrap, params string[] Roles);

    public static class IdentityExt
    {
        public static void Check(this IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray().Join(", ");
                throw new Exception(errors);
            }
        }
    }
}
