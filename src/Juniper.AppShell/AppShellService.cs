using Juniper.TSBuild;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Juniper.AppShell;


/// <summary>
/// A <see cref="BackgroundService"/> that opens a appShell containing a WebView, waits for the
/// <see cref="WebApplication"/> to start, finds the `startup_port` the app started with, and navigates 
/// the WebView to `http://localhost:{startup_port}`
/// </summary>
public class AppShellService : BackgroundService, IAppShellService, IAppShell
{
    private readonly TaskCompletionSource appStarting = new();
    private readonly TaskCompletionSource appStopping = new();
    private readonly CancellationTokenSource serviceCanceller = new();
    private readonly TaskCompletionSource<Uri> addressFetching = new();
    private readonly TaskCompletionSource<IAppShell> appShellCreating = new();
    private readonly IAppShellFactory factory;

    private readonly IServiceProvider services;
    private readonly IOptions<AppShellOptions> options;
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime lifetime;
    private readonly IBuildSystemService? buildSystem;

    public AppShellService(IAppShellFactory factory, IServiceProvider services, IHostApplicationLifetime lifetime, IOptions<AppShellOptions> options, ILogger<AppShellService> logger)
    {
        this.factory = factory;
        this.services = services;
        this.options = options;
        this.logger = logger;
        this.lifetime = lifetime;
        buildSystem = services.GetService<IBuildSystemService>();

        lifetime.ApplicationStarted.Register(() =>
        {
            appStarting.TrySetResult();
        });

        StopOn(lifetime.ApplicationStopping);
        _ = StartAppShellAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken appCancelled)
    {
        try
        {
            StopOn(appCancelled);

            await Task.WhenAny(
                appStarting.Task,
                appStopping.Task
            );

            if (serviceCanceller.IsCancellationRequested)
            {
                return;
            }

            var address = (services
                .GetRequiredService<IServer>()
                .Features
                ?.Get<IServerAddressesFeature>()
                ?.Addresses
                ?.Select(a => new Uri(a))
                ?.Where(a => a.Scheme.StartsWith("http"))
                ?.OrderByDescending(v => v.Scheme)
                ?.FirstOrDefault())
                ?? throw new Exception("Couldn't get any HTTP addresses.");

            addressFetching.TrySetResult(address);
        }
        catch (Exception exp)
        {
            logger.LogError(exp, "Couldn't get startup address");
            addressFetching.TrySetException(exp);
        }
    }

    private void StopOn(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() =>
        {
            serviceCanceller.Cancel();
        });

        cancellationToken.Register(() =>
        {
            appStopping.TrySetResult();
        });

        cancellationToken.Register(() =>
        {
            addressFetching.TrySetCanceled();
        });

        cancellationToken.Register(() =>
        {
            appShellCreating.TrySetCanceled();
        });
    }

    private async Task StartAppShellAsync()
    {
        try
        {
            var title = options.Value.Window?.Title;
            var maximize = options.Value.Window?.Maximized;
            var width = options.Value.Window?.Size?.Width;
            var height = options.Value.Window?.Size?.Height;
            var splash = options.Value.SplashScreenPath;
            var applicationURIString = options.Value.ApplicationURI;
            var applicationURI = applicationURIString is not null
                ? new Uri(applicationURIString)
                : null;

            var address = await addressFetching.Task;

            if (buildSystem is not null)
            {
                await buildSystem.Started;
            }

            logger.LogInformation("Opening AppShell");
            var appShell = await factory.StartAsync(serviceCanceller.Token);
            _ = appStopping.Task.ContinueWith((_) =>
            {
                _ = appShell.CloseAsync();
            });

            if (title is not null)
            {
                logger.LogInformation("Setting window title: \"{title}\"", title);
                await appShell.SetTitleAsync(title);
            }

            if (splash is not null)
            {
                var splashURI = new Uri(address, splash);
                logger.LogInformation("Showing splash page {URI}", splashURI);
                await appShell.SetSourceAsync(splashURI);
            }

            appShellCreating.TrySetResult(appShell);

            if (buildSystem is not null)
            {
                await buildSystem.Ready;
            }

            var finalURI = applicationURI ?? address;
            logger.LogInformation("Showing final page {URI}", finalURI);
            await appShell.SetSourceAsync(finalURI);

            if (maximize is not null)
            {
                logger.LogInformation("Maximizing window");
                await appShell.MaximizeAsync();
            }
            else if (width is not null && height is not null)
            {
                logger.LogInformation("Setting window size {width} x {height}", width.Value, height.Value);
                await appShell.SetSizeAsync(width.Value, height.Value);
            }

            logger.LogInformation("AppShell ready");
            await appShell.WaitForCloseAsync();

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


    public Task<Uri> GetSourceAsync() =>
        Do(appShell => appShell.GetSourceAsync());

    public Task SetSourceAsync(Uri value) =>
        Do(appShell => appShell.SetSourceAsync(value));

    public Task<string> GetTitleAsync() =>
        Do(appShell => appShell.GetTitleAsync());

    public Task SetTitleAsync(string title) =>
        Do(appShell => appShell.SetTitleAsync(title));

    public Task<bool> GetCanGoBackAsync() =>
        Do(appShell => appShell.GetCanGoBackAsync());

    public Task<bool> GetCanGoForwardAsync() =>
        Do(appShell => appShell.GetCanGoForwardAsync());

    public Task SetSizeAsync(int width, int height) =>
        Do(appShell => appShell.SetSizeAsync(width, height));

    public Task CloseAsync() =>
        Do(appShell => appShell.CloseAsync());

    public Task WaitForCloseAsync() =>
        Do(appShell => appShell.WaitForCloseAsync());

    public Task MaximizeAsync() =>
        Do(appShell => appShell.MaximizeAsync());

    public Task MinimizeAsync() =>
        Do(appShell => appShell.MinimizeAsync());

    public Task<bool> ToggleExpandedAsync() =>
        Do(appShell => appShell.ToggleExpandedAsync());
}
