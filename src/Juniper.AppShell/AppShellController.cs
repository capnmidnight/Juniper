using Microsoft.AspNetCore.Mvc;

namespace Juniper.AppShell
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppShellController : Controller
    {
        private readonly IAppShell? appShell;

        public AppShellController(IServiceProvider services)
        {
            appShell = services.GetService(typeof(IAppShell)) as IAppShell;
        }

        [HttpGet]
        [Route("title")]
        public Task<string> GetTitleAsync() =>
            appShell?.GetTitleAsync()
            ?? Task.FromException<string>(new Exception("The appShell was not started"));

        [HttpPost]
        [Route("title")]
        public Task SetTitle([FromBody] string title) =>
            appShell?.SetTitleAsync(title)
            ?? Task.FromException(new Exception("The appShell was not started"));

        [HttpPost]
        [Route("action")]
        public async Task<bool> Act([FromBody] string actionName)
        {
            if (appShell is not null)
            {
                switch (actionName)
                {
                    case "minimize": await appShell.MinimizeAsync(); return false;
                    case "maximize": return await appShell.ToggleExpandedAsync();
                }
            }

            return false;
        }
    }
}
