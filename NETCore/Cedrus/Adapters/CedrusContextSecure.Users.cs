using System.Security.Claims;

using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<CedrusUser> Users => insecure.Users;

    public IQueryable<CedrusUserRole> UserRoles => insecure.UserRoles;

    public CedrusUserRole AddUserToRole(CedrusUser user, CedrusRole role, CedrusUser adminUser)
    {
        adminUser.AssertAdmin();
        var userRole = user.UserRoles.FirstOrDefault(ur => ur.Role.Id == role.Id);
        if (userRole is null)
        {
            insecure.UserRoles.Add(userRole = new CedrusUserRole
            {
                Role = role,
                User = user
            });
        }
        return userRole;
    }

    public async Task<CedrusUser> GetUserAsync(int userId) =>
        await Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new FileNotFoundException($"User:{userId}");

    public void DeleteUser(CedrusUser user, CedrusUser adminUser)
    {
        adminUser.AssertAdmin();
        insecure.Users.Remove(user);
    }

    public async Task RemoveUserFromRoleAsync(CedrusUser user, CedrusRole role, CedrusUser adminUser)
    {
        adminUser.AssertAdmin();
        if (user == adminUser && role == AdminRole)
        {
            throw new InvalidOperationException("An admin user cannot remove themselves from the Admin role.");
        }

        await userManager.RemoveFromRoleAsync(user, role.Name
            ?? throw new ArgumentException("No role name given", nameof(role)));
    }

#if DEBUG
    public async Task<CedrusUser> XXX_MAKE_DEFAULT_ADMIN_XXX(string userName, string email, Claim primarySID)
    {
        var user = await SetUserAsync(userName, email, primarySID);

        if (!user.IsUser)
        {
            insecure.UserRoles.Add(new CedrusUserRole
            {
                Role = UserRole,
                User = user
            });
        }

        if (!user.IsAdmin)
        {
            insecure.UserRoles.Add(new CedrusUserRole
            {
                Role = AdminRole,
                User = user
            });
        }

        return user;
    }
#endif

    public async Task<CedrusUser> SetUserAsync(string userName, string email)
    {
        email = email.ToLowerInvariant();
        var normalizedEmail = email.ToUpperInvariant();
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        if (user is null)
        {
            var result = await userManager.CreateAsync(user = new CedrusUser
            {
                UserName = userName,
                Email = email,
                NormalizedEmail = normalizedEmail,
                EmailConfirmed = true,
                LockoutEnabled = false
            });
            result.Check();
        }
        else
        {
            user.UserName = userName;
            user.NormalizedUserName = userName.ToUpper();
            user.Email = email;
            user.NormalizedEmail = email.ToUpper();
        }

        return user;
    }

    public async Task<CedrusUser> SetUserAsync(string userName, string email, Claim primarySID)
    {
        var user = await SetUserAsync(userName, email);

        if (primarySID is not null)
        {
            var claims = await userManager.GetClaimsAsync(user);
            var claim = claims.FirstOrDefault(c => c.Type == primarySID.Type
                && c.ValueType == primarySID.ValueType
                && c.Value == primarySID.Value);
            if (claim is null)
            {
                var result = await userManager.AddClaimAsync(user, primarySID);
                result.Check();
            }
        }

        return user;
    }

    public CedrusUser[] GetNewUsers(CedrusUser adminUser)
    {
        adminUser.AssertAdmin();
        return (from user in Users.AsEnumerable()
                where !user.IsUser
                select user)
                .ToArray();
    }

    public Task<IList<CedrusUser>> GetUsersInRoleAsync(string roleName) =>
        userManager.GetUsersInRoleAsync(roleName);
}
