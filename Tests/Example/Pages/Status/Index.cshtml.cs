using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Juniper.Cedrus.Example.Pages.Status;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public int? TrappedStatusCode { get; private set; }
    public string? TrappedPath { get; private set; }

    public void OnGet([FromRoute] int? statusCode, [FromQuery] string? path)
    {
        TrappedStatusCode = statusCode;
        TrappedPath = path;
    }
}
