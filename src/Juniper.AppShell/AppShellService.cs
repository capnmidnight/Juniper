using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<AppShellService<AppShellFactoryT>> logger;

    public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<AppShellService<AppShellFactoryT>> logger)
    {
        this.services = services;
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

            logger.LogInformation("Waiting for server to start");
            await Task.WhenAny(
                appStarting.Task,
                appStopping.Task
            );

            if (serviceCanceller.IsCancellationRequested)
                return;

            logger.LogInformation("Checking addresses");
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

            logger.LogInformation("Starting with address: {address}", address);
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


    public async Task StartAppShellAsync(string title, string splashPage)
    {
        try
        {
            logger.LogInformation("Waiting for local address");
            var address = await addressFetching.Task;

            logger.LogInformation("Opening AppShell");
            var appShell = await factory.StartAsync(serviceCanceller.Token);
            _ = appStopping.Task.ContinueWith((_) =>
            {
                _ = appShell.CloseAsync();
            });

            logger.LogInformation("Showing first page: {splashPage}", splashPage);
            await Task.WhenAll(
                appShell.SetTitleAsync(title),
                appShell.SetSourceAsync(new Uri(address, splashPage))
            );

            logger.LogInformation("AppShell ready");
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
            logger.LogInformation("Getting address again");
            var address = await addressFetching.Task;

            logger.LogInformation("Getting AppShell");
            var appShell = await appShellCreating.Task;

            logger.LogInformation("Running AppShell for real");
            await appShell.SetSourceAsync(address);

            logger.LogInformation("Waiting for AppShell to close");
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

    public Task CloseAsync() =>
        Do(appShell => appShell.CloseAsync());

    public Task WaitForCloseAsync() =>
        Do(appShell => appShell.WaitForCloseAsync());
}
