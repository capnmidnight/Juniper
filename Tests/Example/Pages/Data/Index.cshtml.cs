using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Example.Pages.Data;

public class IndexModel : BaseDBPageModel
{
    public IndexModel(IServiceProvider services)
        : base(services)
    {
    }

    public IActionResult OnGet() =>
        WithUser(user => {
            user.AssertAnyRole("Admin", "Contributor");
            return Page();
        });
}
