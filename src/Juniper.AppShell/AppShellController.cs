using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.AppShell
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppShellController : Controller, IAppShell
    {
        private readonly IAppShell? appShell;

        public AppShellController(IServiceProvider services)
        {
            appShell = services.GetService<IAppShell>();
        }

        [HttpPost("action")]
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

        private Task Do(Func<IAppShell, Task> func)
        {
            if(appShell is null)
            {
                throw new Exception("AppShell is not available");
            }

            return func(appShell);
        }

        private Task<T> Do<T>(Func<IAppShell, Task<T>> func)
        {
            if (appShell is null)
            {
                throw new Exception("AppShell is not available");
            }

            return func(appShell);
        }

        [HttpGet("title")]
        public Task<string> GetTitleAsync() =>
            Do(appShell => appShell.GetTitleAsync());

        [HttpPost("title")]
        public Task SetTitleAsync([FromBody] string title) =>
            Do(appShell => appShell.SetTitleAsync(title));

        [HttpGet("source")]
        public Task<Uri> GetSourceAsync() =>
            Do(appShell => appShell.GetSourceAsync());

        [HttpPost("source")]
        public Task SetSourceAsync([FromBody] Uri source) =>
            Do(appShell => appShell.SetSourceAsync(source));

        [HttpGet("cangoback")]
        public Task<bool> GetCanGoBackAsync() =>
            Do(appShell => appShell.GetCanGoBackAsync());

        [HttpGet("cangoforward")]
        public Task<bool> GetCanGoForwardAsync() =>
            Do(appShell => appShell.GetCanGoForwardAsync());

        Task IAppShell.SetSizeAsync(int width, int height) =>
            Do(appShell => appShell.SetSizeAsync(width, height));

        [HttpPost("size")]
        public async Task<IActionResult> SetSizeAsync([FromBody] string sizeExpr)
        {
            var parts = sizeExpr.Split('x');
            if(appShell is null
                || parts.Length != 2
                || !int.TryParse(parts[0], out var width)
                || !int.TryParse(parts[1], out var height)) {
                return BadRequest();
            }

            await appShell.SetSizeAsync(width, height);
            return Ok();
        }

        [HttpPost("maximize")]
        public Task MaximizeAsync() =>
            Do(appShell => appShell.MaximizeAsync());

        [HttpPost("minimize")]
        public Task MinimizeAsync() =>
            Do(appShell => appShell.MinimizeAsync());

        [HttpPost("toggleexpanded")]
        public Task<bool> ToggleExpandedAsync() =>
            Do(appShell => appShell.ToggleExpandedAsync());

        public Task CloseAsync() =>
            Do(appShell => appShell.CloseAsync());

        Task IAppShell.WaitForCloseAsync() =>
            Do(appShell => appShell.WaitForCloseAsync());

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
}
