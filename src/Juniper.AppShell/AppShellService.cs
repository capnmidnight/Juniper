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
/// <typeparam name="AppShellFactoryT">A concrete instance of <see cref="IAppShellFactory"/> that creates the desired WebView container appShell.</typeparam>
public class AppShellService<AppShellFactoryT> : BackgroundService, IAppShellService, IAppShell
    where AppShellFactoryT : IAppShellFactory, new()
{
    private readonly TaskCompletionSource appStarting = new();
    private readonly TaskCompletionSource appStopping = new();
    private readonly CancellationTokenSource serviceCanceller = new();
    private readonly TaskCompletionSource<Uri> addressFetching = new();
    private readonly TaskCompletionSource<IAppShell> appShellCreating = new();
    private readonly AppShellFactoryT factory = new();

    private readonly IServiceProvider services;
    private readonly IOptions<AppShellOptions> options;
    private readonly ILogger<AppShellService<AppShellFactoryT>> logger;

    public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, IOptions<AppShellOptions> options, ILogger<AppShellService<AppShellFactoryT>> logger)
    {
        this.services = services;
        this.options = options;
        this.logger = logger;
        lifetime.ApplicationStarted.Register(() =>
        {
            appStarting.TrySetResult();
        });

        StopOn(lifetime.ApplicationStopping);
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

    public async Task StartAppShellAsync()
    {
        try
        {
            var title = options.Value.Window?.Title;
            var splash = options.Value.SplashScreenPath;

            var address = await addressFetching.Task;

            logger.LogInformation("Opening AppShell");
            var appShell = await factory.StartAsync(serviceCanceller.Token);
            _ = appStopping.Task.ContinueWith((_) =>
            {
                _ = appShell.CloseAsync();
            });

            if (title is not null)
            {
                await appShell.SetTitleAsync(title);
            }

            if (splash is not null)
            {
                logger.LogInformation("Showing first page ({address}) titled \"{title}\"", address, splash);
                var splashAddress = new Uri(address, splash);
                await appShell.SetSourceAsync(splashAddress);
            }

            appShellCreating.TrySetResult(appShell);
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

    public async Task RunAppShellAsync()
    {
        try
        {
            var width = options.Value.Window?.Size?.Width;
            var height = options.Value.Window?.Size?.Height;
            var address = await addressFetching.Task;
            var appShell = await appShellCreating.Task;
            if (width is not null && height is not null)
            {
                await appShell.SetSize(width.Value, height.Value);
            }

            await appShell.SetSourceAsync(address);

            logger.LogInformation("AppShell ready");
            await appShell.WaitForCloseAsync();
        }
        catch (TaskCanceledException)
        {
            // do nothing
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to run AppShell");
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

    public Task SetSize(int width, int height) =>
        Do(appShell => appShell.SetSize(width, height));

    public Task CloseAsync() =>
        Do(appShell => appShell.CloseAsync());

    public Task WaitForCloseAsync() =>
        Do(appShell => appShell.WaitForCloseAsync());

    public Task SetIconAsync(Uri path) =>
        Do(appShell => appShell.SetIconAsync(path));
}
