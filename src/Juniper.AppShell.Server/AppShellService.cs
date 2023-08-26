using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Juniper.AppShell
{
    public class AppShellService<AppShellWindowFactoryT> : BackgroundService, IAppShell
        where AppShellWindowFactoryT : IAppShellFactory, new()
    {
        private readonly TaskCompletionSource applicationStart = new();
        private readonly AppShellWindowFactoryT factory = new();

        private readonly IServiceProvider services;
        private readonly IHostApplicationLifetime lifetime;
        private readonly ILogger<AppShellService<AppShellWindowFactoryT>> logger;

        private readonly Task<IAppShell> windowTask;

        public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<AppShellService<AppShellWindowFactoryT>> logger)
        {
            this.services = services;
            this.lifetime = lifetime;
            this.logger = logger;
            this.lifetime.ApplicationStarted.Register(applicationStart.SetResult);
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

                var addresses = services
                    .GetRequiredService<IServer>()
                    ?.Features
                    ?.Get<IServerAddressesFeature>()
                    ?.Addresses
                    ?.ToArray();

                if (addresses is null || addresses.Length == 0)
                {
                    throw new Exception("Couldn't get addresses.");
                }

                logger.LogInformation("Addresses: {addresses}", string.Join(", ", addresses));

                var address = addresses
                    .Where(v => v.StartsWith("http:"))
                    .Select(v => v.Replace("[::]", "localhost"))
                    .FirstOrDefault()
                    ?? addresses.First();

                logger.LogInformation("Selected address: {address} (current source: {source}", address, await GetSourceAsync());
                await SetSourceAsync(new Uri(address));
            }
            catch(Exception exp)
            {
                logger.LogError(exp, "Could not start app window");
                await CloseAsync();
            }
        }

        public async Task<Uri> GetSourceAsync() {
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
