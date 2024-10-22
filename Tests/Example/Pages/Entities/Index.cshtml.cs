using Juniper.Cedrus.Entities;

using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Example.Pages.Entities;

public class IndexModel : BaseDBPageModel
{
    public EntityType[]? EntityTypes;
    public bool? IsContributor;

    public IndexModel(IServiceProvider services)
        : base(services)
    {
    }

    public IActionResult OnGet() =>
        WithUser((user) =>
        {
            user.AssertUser();
            
            IsContributor = user.HasRole("Contributor");

            EntityTypes = Database.GetEntityTypesWithEntities(user)
                .Where(et => et.IsPrimary)
                .AsEnumerable()
                .Where(et => et.Parent is null 
                    || (et.IsPrimary && !et.Parent.IsPrimary))
                .ToArray();

            return Page();
        });


}
