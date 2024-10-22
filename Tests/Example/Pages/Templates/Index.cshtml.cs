using Juniper.Cedrus.Entities;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Example.Pages.Templates;

public class IndexModel : BaseDBPageModel
{
    public EntityType[]? EntityTypes;

    public IndexModel(IServiceProvider services)
        : base(services)
    {
    }

    public IActionResult OnGet() =>
        WithUser((user) =>
        {
            user.AssertRole("Contributor");

            EntityTypes = Database.FindEntityTypes()
                .AsEnumerable()
                .Where(et => et.Parent is null)
                .ToArray();

            return Page();
        });


}
