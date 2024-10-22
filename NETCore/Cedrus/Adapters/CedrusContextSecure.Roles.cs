using Juniper.Cedrus.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    public IQueryable<CedrusRole> Roles => insecure.Roles;

    public async Task<CedrusRole> GetRoleAsync(int roleId) =>
        await Roles.FirstOrDefaultAsync(u => u.Id == roleId)
            ?? throw new FileNotFoundException($"Role:{roleId}");


    [SeedValue]
    public CedrusRole AdminRole => Lazy(() => CreateRoleInternalAsync("Admin"));

    [SeedValue]
    public CedrusRole UserRole => Lazy(() => CreateRoleInternalAsync("User"));

    public Task<CedrusRole> CreateRoleAsync(string roleName, CedrusUser admin)
    {
        admin.AssertAdmin();
        return CreateRoleInternalAsync(roleName);
    }

    private async Task<CedrusRole> CreateRoleInternalAsync(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            var result = await roleManager.CreateAsync(role = new CedrusRole
            {
                Name = roleName
            });
            result.Check();
        }

        return role;
    }
}
