using Microsoft.AspNetCore.Mvc;

namespace Juniper.Cedrus.Example.Pages.Tests.DeliberateError;

public class IndexModel : BaseDBPageModel
{
    public IndexModel(IServiceProvider services)
        : base(services)
    {
    }

    public IActionResult OnGet() =>
        throw new NotImplementedException();

}
