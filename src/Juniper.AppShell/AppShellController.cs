using Microsoft.AspNetCore.Mvc;

namespace Juniper.AppShell
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppShellController : Controller
    {
        private readonly IAppShell appShell;

        public AppShellController(IAppShell appShell)
        {
            this.appShell = appShell;
        }

        [HttpGet]
        [Route("title")]
        public Task<string> GetTitleAsync() =>
            appShell.GetTitleAsync();

        [HttpPost]
        [Route("title")]
        public Task SetTitle([FromBody] string title) => 
            appShell.SetTitleAsync(title);
    }
}
