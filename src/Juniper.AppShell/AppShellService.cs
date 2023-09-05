using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Threading;

namespace Juniper.AppShell;

/// <summary>
/// A <see cref="BackgroundService"/> that opens a Window containing a WebView, waits for the
/// <see cref="WebApplication"/> to start, finds the `startup_port` the app started with, and navigates 
/// the WebView to `http://localhost:{startup_port}`
/// </summary>
/// <typeparam name="AppShellWindowFactoryT">A concrete instance of <see cref="IAppShellFactory"/> that creates the desired WebView container Window.</typeparam>
public class AppShellService<AppShellWindowFactoryT> : IAppShellService, IAppShell
    where AppShellWindowFactoryT : IAppShellFactory, new()
{
    private readonly TaskCompletionSource<Uri> addressTask = new();
    private readonly IServiceProvider services;
    private readonly ILogger<AppShellService<AppShellWindowFactoryT>> logger;
    private readonly Task<IAppShell> appShellTask;

    public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<AppShellService<AppShellWindowFactoryT>> logger)
    {
        this.services = services;
        this.logger = logger;
        appShellTask = new AppShellWindowFactoryT().StartAsync();
        lifetime.ApplicationStarted.Register(GetAddress);
        logger.LogInformation("Opening window");
    }

    private void GetAddress()
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
        catch(Exception exp)
        {
            logger.LogError(exp, "Couldn't get startup address");
            addressTask.SetException(exp);
        }
    }

    public async Task StartAsync(string splashPage)
    {
        logger.LogInformation("Showing first page: {slashPage}", splashPage);
        var address = await addressTask.Task;
        await SetSourceAsync(new Uri(address, splashPage));
    }

    public async Task<IAppShell> RunAsync()
    {
        logger.LogInformation("Running app shell for real");
        var address = await addressTask.Task;
        await SetSourceAsync(address);
        return await appShellTask;
    }

    private async Task Do(Func<IAppShell, Task> action)
    {
        var window = await appShellTask;
        await action(window);
    }

    private async Task<T> Do<T>(Func<IAppShell, Task<T>> action)
    {
        var window = await appShellTask;
        return await action(window);
    }

    public Task<Uri> GetSourceAsync() =>
        Do(window => window.GetSourceAsync());

    public Task SetSourceAsync(Uri value) =>
        Do(window => window.SetSourceAsync(value));

    public Task<string> GetTitleAsync() =>
        Do(window => window.GetTitleAsync());

    public Task SetTitleAsync(string title) =>
        Do(window => window.SetTitleAsync(title));

    public Task CloseAsync() =>
        Do(window => window.CloseAsync());

    public Task WaitForCloseAsync() =>
        Do(window => window.WaitForCloseAsync());
}
