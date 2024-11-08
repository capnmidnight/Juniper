using Juniper.Services;
using Juniper.TSBuild;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Juniper.AppShell;


/// <summary>
/// A singleton that opens a appShell containing a WebView, waits for the
/// <see cref="Microsoft.AspNetCore.Builder.WebApplication"/> to start, finds the `startup_port` the app started with, and navigates 
/// the WebView to `http://localhost:{startup_port}`
/// </summary>
public class AppShellService : IAppShellService, IAppShell
{
    private readonly TaskCompletionSource appStarting = new();
    private readonly TaskCompletionSource appStopping = new();
    private readonly CancellationTokenSource serviceCanceller = new();
    private readonly TaskCompletionSource<IAppShell> appShellCreating = new();
    private readonly TaskCompletionSource ready = new();
    private readonly IAppShellFactory factory;
    private readonly AppShellOptions options;
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime lifetime;
    private readonly IPortDiscoveryService portDiscovery;
    private readonly IBuildSystemService? buildSystem;

    public Task Ready => ready.Task;
    public Task Complete { get; }

    public AppShellService(IAppShellFactory factory, IServiceProvider services, IHostApplicationLifetime lifetime, IOptions<AppShellOptions> options, ILogger<AppShellService> logger, IPortDiscoveryService portDiscovery)
    {
        this.factory = factory;
        this.options = options.Value;
        this.logger = logger;
        this.lifetime = lifetime;
        this.portDiscovery = portDiscovery;
        buildSystem = services.GetService<IBuildSystemService>();

        lifetime.ApplicationStarted.Register(() =>
        {
            appStarting.TrySetResult();
        });

        StopOn(lifetime.ApplicationStopping);

        Complete = StartAsync(serviceCanceller.Token);
    }

    public bool OwnsRunning => factory.OwnsRunning;
    public void Run() => factory.Run();

    private void StopOn(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() =>
        {
            serviceCanceller.Cancel();
            appStopping.TrySetResult();
            appShellCreating.TrySetCanceled();
        });
    }

    private async Task StartAsync(CancellationToken stoppingToken)
    {
        try
        {
            var title = options.Window?.Title;
            var fullscreen = options.Window?.Fullscreen;
            var borderless = options.Window?.Borderless;
            var maximized = options.Window?.Maximized;
            var width = options.Window?.Size?.Width;
            var height = options.Window?.Size?.Height;
            var splash = options.SplashScreenPath;
            var applicationURIString = options.ApplicationURI;
            var applicationURI = applicationURIString is not null
                ? new Uri(applicationURIString)
                : null;

            logger.LogInformation("Opening AppShell");
            var appShell = await factory.StartAsync(serviceCanceller.Token);

            _ = appStopping.Task.ContinueWith(delegate { appShell.CloseAsync(); }, stoppingToken);

            if (title is not null)
            {
                logger.LogInformation("Setting window title: \"{title}\"", title);
                await appShell.SetTitleAsync(title);
            }

            if (fullscreen is not null)
            {
                logger.LogInformation("Setting window fullscreen: \"{fullscreen}\"", fullscreen);
                await appShell.SetIsFullscreenAsync(fullscreen.Value);
            }

            if (splash is not null && File.Exists(splash))
            {
                await appShell.SetIsBorderlessAsync(true);
                borderless ??= false;

                var splashHTML = File.ReadAllText(splash);
                logger.LogInformation("Showing splash page");
                await appShell.SetSourceHTMLAsync(splashHTML);
            }

            appShellCreating.TrySetResult(appShell);
            ready.TrySetResult();

            if (buildSystem is not null)
            {
                await buildSystem.Ready;
                buildSystem.NewBuildCompleted += delegate
                {
                    appShell.ReloadAsync();
                };
            }

            if (borderless is not null && borderless != await appShell.GetIsBorderlessAsync())
            {
                logger.LogInformation("Setting window borderless: \"{borderless}\"", borderless);
                await appShell.SetIsBorderlessAsync(borderless.Value);
            }

            var finalURI = applicationURI ?? await portDiscovery.GettingAddress;
            logger.LogInformation("Showing final page {URI}", finalURI);
            await appShell.SetSourceUriAsync(finalURI);

            if (maximized is not null)
            {
                logger.LogInformation("Maximizing window");
                await appShell.SetIsMaximizedAsync(maximized.Value);
            }
            else if (width is not null && height is not null)
            {
                logger.LogInformation("Setting window size {width} x {height}", width.Value, height.Value);
                await appShell.SetSizeAsync(width.Value, height.Value);
            }

            logger.LogInformation("AppShell ready");
            await appShell.WaitForCloseAsync();

            logger.LogInformation("AppShell closing");
            if (!lifetime.ApplicationStopping.IsCancellationRequested)
            {
                lifetime.StopApplication();
            }

        }
        catch (TaskCanceledException)
        {
            // do nothing
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to start AppShell");
        }
    }

    private async Task Do(Func<IAppShell, Task> action) =>
        await action(await appShellCreating.Task);


    private async Task<T> Do<T>(Func<IAppShell, Task<T>> action) =>
        await action(await appShellCreating.Task);

    /////////////////
    //// CLOSING ////
    /////////////////

    public Task ShowAsync() =>
        Do(appShell => appShell.ShowAsync());

    public Task HideAsync() =>
        Do(appShell => appShell.HideAsync());

    public Task CloseAsync() =>
        Do(appShell => appShell.CloseAsync());

    public Task WaitForCloseAsync() =>
        Do(appShell => appShell.WaitForCloseAsync());

    ////////////////
    //// SOURCE ////
    ////////////////

    public Task<Uri> GetSourceUriAsync() =>
        Do(appShell => appShell.GetSourceUriAsync());

    public Task SetSourceUriAsync(Uri source) =>
        Do(appShell => appShell.SetSourceUriAsync(source));
    public Task SetSourceHTMLAsync(string html) =>
        Do(appShell => appShell.SetSourceHTMLAsync(html));

    public Task ReloadAsync() =>
        Do(appShell => appShell.ReloadAsync());

    ///////////////
    //// TITLE ////
    ///////////////

    public Task<string> GetTitleAsync() =>
        Do(appShell => appShell.GetTitleAsync());

    public Task SetTitleAsync(string title) =>
        Do(appShell => appShell.SetTitleAsync(title));

    /////////////////
    //// HISTORY ////
    /////////////////

    public Task<bool> GetCanGoBackAsync() =>
        Do(appShell => appShell.GetCanGoBackAsync());

    public Task<bool> GetCanGoForwardAsync() =>
        Do(appShell => appShell.GetCanGoForwardAsync());

    //////////////
    //// SIZE ////
    //////////////

    public Task<Size> GetSizeAsync() =>
        Do(appShell => appShell.GetSizeAsync());

    public Task SetSizeAsync(int width, int height) =>
        Do(appShell => appShell.SetSizeAsync(width, height));

    ////////////////////
    //// FULLSCREEN ////
    ////////////////////

    public Task<bool> GetIsFullscreenAsync() =>
        Do(appShell => appShell.GetIsFullscreenAsync());

    public Task SetIsFullscreenAsync(bool isFullscreen) =>
        Do(appShell => appShell.SetIsFullscreenAsync(isFullscreen));

    ////////////////////
    //// BORDERLESS ////
    ////////////////////

    public Task<bool> GetIsBorderlessAsync() =>
        Do(appShell => appShell.GetIsBorderlessAsync());

    public Task SetIsBorderlessAsync(bool isBorderless) =>
        Do(appShell => appShell.SetIsBorderlessAsync(isBorderless));

    ///////////////////
    //// MAXIMIZED ////
    ///////////////////

    public Task<bool> GetIsMaximizedAsync() =>
        Do(appShell => appShell.GetIsMaximizedAsync());

    public Task SetIsMaximizedAsync(bool isMaximized) =>
        Do(appShell => appShell.SetIsMaximizedAsync(isMaximized));

    ///////////////////
    //// MINIMIZED ////
    ///////////////////

    public Task<bool> GetIsMinimizedAsync() =>
        Do(appShell => appShell.GetIsMinimizedAsync());

    public Task SetIsMinimizedAsync(bool isMinimized) =>
        Do(appShell => appShell.SetIsMinimizedAsync(isMinimized));
}
