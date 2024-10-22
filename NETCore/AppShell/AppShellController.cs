using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.AppShell;

/// <summary>
/// REST API endpoint for interfacing with the AppShell
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AppShellController : Controller, IBaseAppShell
{
    private readonly IAppShell? appShell;

    public AppShellController(IServiceProvider services)
    {
        appShell = services.GetService<IAppShell>();
    }

    private Task Do(Func<IAppShell, Task> func) =>
        func(appShell ?? throw new Exception("AppShell is not available"));

    private Task<T> Do<T>(Func<IAppShell, Task<T>> func) =>
        func(appShell ?? throw new Exception("AppShell is not available"));

    /////////////////
    //// CLOSING ////
    /////////////////

    [HttpPost("show")]
    public Task ShowAsync() =>
        Do(appShell => appShell.ShowAsync());

    [HttpPost("hide")]
    public Task HideAsync() =>
        Do(appShell => appShell.HideAsync());

    [HttpPost("close")]
    public Task CloseAsync() =>
        Do(appShell => appShell.CloseAsync());

    ///////////////
    //// TITLE ////
    ///////////////

    [HttpGet("title")]
    public Task<string> GetTitleAsync() =>
        Do(appShell => appShell.GetTitleAsync());

    [HttpPost("title")]
    public Task SetTitleAsync([FromBody] string title) =>
        Do(appShell => appShell.SetTitleAsync(title));

    /////////////////
    //// HISTORY ////
    /////////////////

    private Task<bool> MaybeDo(Func<IAppShell, Task<bool>> func)
    {
        if (appShell is null)
        {
            return Task.FromResult(false);
        }

        return func(appShell);
    }

    [HttpGet("cangoback")]
    public Task<bool> GetCanGoBackAsync() =>
        MaybeDo(appShell => appShell.GetCanGoBackAsync());

    [HttpGet("cangoforward")]
    public Task<bool> GetCanGoForwardAsync() =>
        MaybeDo(appShell => appShell.GetCanGoForwardAsync());

    //////////////
    //// SIZE ////
    //////////////

    Task<Size> IBaseAppShell.GetSizeAsync() =>
        Do(appShell => appShell.GetSizeAsync());

    [HttpGet("size")]
    public Task<string> GetSizeAsync() =>
        Do(async appShell =>
        {
            var size = await appShell.GetSizeAsync();
            return $"{size.Width}x{size.Height}";
        });

    Task IBaseAppShell.SetSizeAsync(int width, int height) =>
        Do(appShell => appShell.SetSizeAsync(width, height));

    [HttpPost("size")]
    public async Task<IActionResult> SetSizeAsync([FromBody] string sizeExpr)
    {
        var parts = sizeExpr.Split('x');
        if (appShell is null
            || parts.Length != 2
            || !int.TryParse(parts[0], out var width)
            || !int.TryParse(parts[1], out var height))
        {
            return BadRequest();
        }

        await appShell.SetSizeAsync(width, height);
        return Ok();
    }

    ////////////////////
    //// FULLSCREEN ////
    ////////////////////

    [HttpGet("fullscreen")]
    public Task<bool> GetIsFullscreenAsync() =>
        Do(appShell => appShell.GetIsFullscreenAsync());

    [HttpPost("fullscreen")]
    public Task SetIsFullscreenAsync([FromBody] bool isFullscreen) =>
        Do(appShell => appShell.SetIsFullscreenAsync(isFullscreen));

    ////////////////////
    //// BORDERLESS ////
    ////////////////////

    [HttpGet("borderless")]
    public Task<bool> GetIsBorderlessAsync() =>
        Do(appShell => appShell.GetIsBorderlessAsync());

    [HttpPost("borderless")]
    public Task SetIsBorderlessAsync([FromBody] bool isBorderless) =>
        Do(appShell => appShell.SetIsBorderlessAsync(isBorderless));

    ///////////////////
    //// MAXIMIZED ////
    ///////////////////

    [HttpGet("maximized")]
    public Task<bool> GetIsMaximizedAsync() =>
        Do(appShell => appShell.GetIsMaximizedAsync());

    [HttpPost("maximized")]
    public Task SetIsMaximizedAsync([FromBody] bool isMaximized) =>
        Do(appShell => appShell.SetIsMaximizedAsync(isMaximized));

    ///////////////////
    //// MINIMIZED ////
    ///////////////////

    [HttpGet("minimized")]
    public Task<bool> GetIsMinimizedAsync() =>
        Do(appShell => appShell.GetIsMinimizedAsync());

    [HttpPost("minimized")]
    public Task SetIsMinimizedAsync([FromBody] bool isMinimized) =>
        Do(appShell => appShell.SetIsMinimizedAsync(isMinimized));
}
