using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;
using Juniper.Data;
using Microsoft.AspNetCore.Identity;


#if DEBUG
using System.Security.Claims;
#else
using Microsoft.EntityFrameworkCore;
#endif

namespace Juniper.Cedrus.Example.Models;

public static partial class DataSeeder
{
    [DataSeeder]
    public static async Task SeedData(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var db = services.GetRequiredService<CedrusContextSecure>();

#if DEBUG
        var localUser = ClaimsPrincipal.Current;
        var primarySID = localUser?.FindFirst(ClaimTypes.PrimarySid)
            ?? new Claim(ClaimTypes.PrimarySid, "example");
        var testUser = Environment.UserName;
        var testEmail = "unknown@example.com";
        var user = await db.XXX_MAKE_DEFAULT_ADMIN_XXX(testUser, testEmail, primarySID);

#else
        var user = await db.Roles
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .Where(r => r.Name == "Admin")
            .SelectMany(r => r.UserRoles)
            .Select(ur => ur.User)
            .FirstOrDefaultAsync()
            ?? throw new FileNotFoundException("Could not find an admin user for which to perform the data seeding.");
#endif

        var contributorRole = await db.CreateRoleAsync("Contributor", user);
#if DEBUG
        var userMgr = services.GetRequiredService<UserManager<CedrusUser>>();
        await userMgr.SetRoleAsync(user, contributorRole.Name!);
#endif

        var namedObjectET = await db.SetEntityTypeAsync("Named Object", false);
        var systemET = await db.SetEntityTypeAsync("System", true, namedObjectET);

        var imagePT = await db.SetPropertyTypeAsync(DataType.File, "Image", "A photo of the system.");

        await db.SetPropertyTemplateAsync(
            namedObjectET,
            "Info",
            db.NamePropertyType,
            db.DescriptionPropertyType
        );

        await db.SetPropertyTemplateAsync(
            systemET,
            "Info",
            imagePT
        );

        await db.SaveChangesAsync();
    }
}
