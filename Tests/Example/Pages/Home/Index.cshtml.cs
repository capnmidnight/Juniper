using Juniper.Cedrus.Entities;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Example.Pages.Home;

[AllowAnonymous]
public class IndexModel : BaseDBPageModel
{
    public IList<CedrusUser> AdminUsers { get; private set; } = [];
    public CedrusUser[] NewUsers { get; private set; } = [];


    public IndexModel(IServiceProvider services)
        : base(services)
    {
    }

    public Task<IActionResult> OnGet() =>
        WithErrorHandlingAsync(async () =>
        {
            AdminUsers = await UserManager.GetUsersInRoleAsync("Admin");

            if (IdentityUser?.IsAdmin ?? false)
            {
                NewUsers = Database.GetNewUsers(IdentityUser);
            }

            return Page();
        });

    public string FormatGrantEmail(CedrusUser admin) =>
        $"mailto:{admin.Email}?subject={Uri.EscapeDataString("New User Request")}&body={Uri.EscapeDataString(@$"Hello, {admin.UserName}.

Please grant me access to Cedrus.

Thank you,
{IdentityUser?.UserName}")}";

}
