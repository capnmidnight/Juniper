using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Juniper.Services;

/// <summary>
/// REST API endpoint for interfacing with the AppShell
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MenuHiderController : Controller
{
    [HttpPost("hidemenu")]
    public void HideMenu([FromBody] string hidden)
    {
        if (hidden == "true")
        {
            Request.HttpContext.Session.SetString("hideMenu", "hidden");
        }
        else
        {
            Request.HttpContext.Session.Remove("hideMenu");
        }
    }
}
