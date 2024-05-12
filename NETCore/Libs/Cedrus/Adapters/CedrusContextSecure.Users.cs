using System.Security.Claims;

using Juniper.Cedrus.Entities;
using Juniper.Data.Identity;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<CedrusUser> Users => insecure.Users;

    public IQueryable<CedrusUserRole> UserRoles => insecure.UserRoles;

    public void AddUserToRole(CedrusUser user, CedrusRole role)
    {
        insecure.UserRoles.Add(new CedrusUserRole
        {
            Role = role,
            User = user
        });
    }

    public CedrusUser GetUser(int userId) =>
        GetUserAsync(userId).Result;

    public async Task<CedrusUser> GetUserAsync(int userId) =>
        await Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new FileNotFoundException();

    public void DeleteUser(CedrusUser user, CedrusUser adminUser)
    {
        AssertAdmin(adminUser);
        insecure.Users.Remove(user);
    }

    public async Task RemoveUserFromRoleAsync(CedrusUser user, CedrusRole role, CedrusUser adminUser)
    {
        AssertAdmin(adminUser);
        await userManager.RemoveFromRoleAsync(user, role.Name 
            ?? throw new ArgumentException("No role name given", nameof(role)));
    }

    public CedrusUser XXX_MAKE_DEFAULT_ADMIN_XXX(string testUser, string testEmail, Claim primarySID, Classification classification)
    {
        var user = userManager.Users.SingleOrDefault(u => u.Email == testEmail);

        if (user is null)
        {
            var result = userManager.CreateAsync(user = new CedrusUser
            {
                UserName = testUser,
                Email = testEmail,
                EmailConfirmed = true,
                LockoutEnabled = false,
                Classification = classification
            }).Result;
            result.Check();

            result = userManager.AddClaimAsync(user, primarySID).Result;
            result.Check();
        }

        if (!user.UserRoles.Any(ur => ur.Role.Name == "Admin"))
        {
            AddUserToRole(user, AdminRole);
        }

        return user;
    }
}
