using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;

namespace Juniper.AppShell;

/// <summary>
/// A <see cref="BackgroundService"/> that opens a appShell containing a WebView, waits for the
/// <see cref="WebApplication"/> to start, finds the `startup_port` the app started with, and navigates 
/// the WebView to `http://localhost:{startup_port}`
/// </summary>
/// <typeparam name="AppShellFactoryT">A concrete instance of <see cref="IAppShellFactory"/> that creates the desired WebView container appShell.</typeparam>
public class AppShellService<AppShellFactoryT> : IAppShellService, IAppShell
    where AppShellFactoryT : IAppShellFactory, new()
{
    private readonly TaskCompletionSource<Uri> addressTask = new();
    private readonly TaskCompletionSource<IAppShell> appShellTask = new();
    private readonly ILogger<AppShellService<AppShellFactoryT>> logger;

    public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<AppShellService<AppShellFactoryT>> logger)
    {
        this.logger = logger;
        lifetime.ApplicationStarted.Register(() =>
        {
            try
            {
                logger.LogInformation("Checking addresses...");

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

                addressTask.SetResult(address);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Couldn't get startup address");
                addressTask.SetException(exp);
            }
        });
    }

    public async Task StartAppShellAsync(string title, string splashPage)
    {
        logger.LogInformation("Opening appShell");
        var appShell = await new AppShellFactoryT().StartAsync();
        logger.LogInformation("Showing first page: {slashPage}", splashPage);
        var address = await addressTask.Task;
        appShellTask.SetResult(appShell);
        await Task.WhenAll(
            SetTitleAsync(title),
            SetSourceAsync(new Uri(address, splashPage))
        );
    }

    public async Task<IAppShell> RunAppShellAsync()
    {
        logger.LogInformation("Running app shell for real");
        var address = await addressTask.Task;
        await SetSourceAsync(address);
        return await appShellTask.Task;
    }

    private async Task Do(Func<IAppShell, Task> action) =>
        await action(await appShellTask.Task);

    private async Task<T> Do<T>(Func<IAppShell, Task<T>> action) =>
        await action(await appShellTask.Task);

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
