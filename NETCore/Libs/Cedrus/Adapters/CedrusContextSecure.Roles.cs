using Juniper.Cedrus.Entities;
using Juniper.Data.Identity;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<CedrusRole> Roles => insecure.Roles;

    public async Task<CedrusRole> GetRoleAsync(int roleId) =>
        await Roles.FirstOrDefaultAsync(u => u.Id == roleId)
            ?? throw new FileNotFoundException();

    public void AddUserToRole(CedrusRole role, CedrusUser user) =>
        AddUserToRole(user, role);

    public CedrusRole AdminRole
    {
        get
        {
            var role = roleManager.FindByNameAsync("Admin").Result;
            if (role is null)
            {
                var result = roleManager.CreateAsync(role = new CedrusRole
                {
                    Name = "Admin"
                }).Result;
                result.Check();
            }

            return role;
        }
    }
}
