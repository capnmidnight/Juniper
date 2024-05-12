using Microsoft.EntityFrameworkCore.Infrastructure;

using Juniper.Cedrus.Entities;
using Microsoft.AspNetCore.Identity;

namespace Juniper.Cedrus.Models;

public partial class CedrusContextSecure
{
    private static string ValidateString(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Argument was null or whitespace", name);
        }

        return value;
    }

    private CedrusContextInsecure insecure;
    private UserManager<CedrusUser> userManager;
    private RoleManager<CedrusRole> roleManager;

    public CedrusContextSecure(CedrusContextInsecure insecure, UserManager<CedrusUser> userManager, RoleManager<CedrusRole> roleManager)
    {
        this.insecure = insecure;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public void AssertAdmin(CedrusUser adminUser)
    {
        if (!adminUser.UserRoles.Any(ur => ur.Role.Id == AdminRole.Id))
        {
            throw new UnauthorizedAccessException();
        }
    }

    public DatabaseFacade Database => insecure.Database;

    public Task<int> SaveChangesAsync() => insecure.SaveChangesAsync();

    public int SaveChanges() => insecure.SaveChanges();
}
