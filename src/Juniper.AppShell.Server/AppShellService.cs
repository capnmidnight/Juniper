using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Juniper.AppShell
{
    /// <summary>
    /// A <see cref="BackgroundService"/> that opens a Window containing a WebView, waits for the
    /// <see cref="WebApplication"/> to start, finds the `startup_port` the app started with, and navigates 
    /// the WebView to `http://localhost:{startup_port}`
    /// </summary>
    /// <typeparam name="AppShellWindowFactoryT">A concrete instance of <see cref="IAppShellFactory"/> that creates the desired WebView container Window.</typeparam>
    public class AppShellService<AppShellWindowFactoryT> : BackgroundService, IAppShell
        where AppShellWindowFactoryT : IAppShellFactory, new()
    {
        private readonly TaskCompletionSource applicationStart = new();
        private readonly AppShellWindowFactoryT factory = new();
        private readonly IServiceProvider services;
        private readonly ILogger<AppShellService<AppShellWindowFactoryT>> logger;

        private readonly Task<IAppShell> windowTask;

        public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<AppShellService<AppShellWindowFactoryT>> logger)
        {
            this.services = services;
            this.logger = logger;
            lifetime.ApplicationStarted.Register(applicationStart.SetResult);
            logger.LogInformation("Opening window");
            windowTask = factory.StartAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var serviceCancellation = new TaskCompletionSource();
                cancellationToken.Register(serviceCancellation.SetResult);

                await Task.WhenAny(serviceCancellation.Task, applicationStart.Task)
                    .ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                logger.LogInformation("Checking addresses...");

                var address = (services
                    .GetRequiredService<IServer>()
                    .Features
                    ?.Get<IServerAddressesFeature>()
                    ?.Addresses
                    ?.Select(a => new Uri(a))
                    ?.Where(a => a.Scheme.StartsWith("http"))
                    ?.OrderByDescending(v => v.Scheme)
                    ?.Select(v => new Uri($"{v.Scheme}://localhost:{v.Port}"))
                    ?.FirstOrDefault())
                    ?? throw new Exception("Couldn't get any HTTP addresses.");

                logger.LogInformation("Starting with address: {address}", address);
                await SetSourceAsync(address);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Could not start app window");
                await CloseAsync();
            }
        }

        public async Task<Uri> GetSourceAsync()
        {
            var window = await windowTask;
            return await window.GetSourceAsync();
        }

        public async Task SetSourceAsync(Uri value)
        {
            var window = await windowTask;
            await window.SetSourceAsync(value);
        }

        public async Task CloseAsync()
        {
            var window = await windowTask;
            await window.CloseAsync();
        }
    }
}
