using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using System.Text;

namespace Juniper.Data
{
    public record BootsrapUser(string Email, bool bootstrap, params string[] roles);

    public static class IdentityExt
    {

        public static async Task<Dictionary<string, IdentityRole>> SeedRoles(this RoleManager<IdentityRole> roleMgr, string[] roles)
        {
            var requiredRoles = new HashSet<string>(roles);
            var roleObjs = roleMgr.Roles
                .AsEnumerable()
                .Where(r => requiredRoles.Contains(r.Name))
                .GroupBy(r => r.Name)
                .ToDictionary(r => r.Key, r => r.First());

            foreach (var roleName in roles)
            {
                if (!roleObjs.ContainsKey(roleName))
                {
                    var role = new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    };

                    await roleMgr.CreateAsync(role);
                    roleObjs.Add(roleName, role);
                }
            }

            return roleObjs;
        }

        public static async Task SeedUsers(this UserManager<IdentityUser> userMgr, BootsrapUser[] emails, RoleManager<IdentityRole> roleMgr, Dictionary<string, IdentityRole> roleObjs, ILogger logger)
        {
            var requiredUsers = new HashSet<string>(emails.Select(e => e.Email).Distinct());
            var users = userMgr.Users
                .AsEnumerable()
                .Where(u => requiredUsers.Contains(u.Email))
                .GroupBy(u => u.Email)
                .ToDictionary(u => u.Key, u => u.First());

            foreach (var (email, bootstrap, roleNames) in emails)
            {
                if (!users.TryGetValue(email, out var user))
                {
                    user = new IdentityUser
                    {
                        UserName = email,
                        NormalizedUserName = email.ToUpperInvariant(),
                        Email = email,
                        NormalizedEmail = email.ToUpperInvariant(),
                        EmailConfirmed = true,
                        LockoutEnabled = false
                    };

                    await userMgr.CreateAsync(user);
                    users.Add(email, user);
                }

                foreach (var roleName in roleNames)
                {
                    if (!roleObjs.ContainsKey(roleName))
                    {
                        var role = new IdentityRole
                        {
                            Name = roleName,
                            NormalizedName = roleName.ToUpperInvariant()
                        };

                        await roleMgr.CreateAsync(role);
                        roleObjs.Add(roleName, role);
                    }

                    var isInRole = await userMgr.IsInRoleAsync(user, roleName);
                    if (!isInRole)
                    {
                        await userMgr.AddToRoleAsync(user, roleName);
                    }
                }

                if (string.IsNullOrEmpty(user.PasswordHash) && bootstrap)
                {
                    var code = await userMgr.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    logger.LogWarning($"Core user {user.Email} does not have a password. Reset it here: /Identity/Account/ResetPassword?code={code}");
                }
            }
        }
    }
}
