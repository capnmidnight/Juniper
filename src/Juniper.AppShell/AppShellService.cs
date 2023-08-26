using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace Juniper.AppShell
{
    public class AppShellService : BackgroundService
    {
        private readonly IServiceProvider services;
        private readonly IHostApplicationLifetime lifetime;
        private readonly ILogger<AppShellService> logger;
        private readonly TaskCompletionSource source = new();

        public AppShellService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<AppShellService> logger)
        {
            this.services = services;
            this.lifetime = lifetime;
            this.logger = logger;

            this.lifetime.ApplicationStarted.Register(source.SetResult);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var serviceCancellation = new TaskCompletionSource();
            cancellationToken.Register(serviceCancellation.SetResult);

            await Task.WhenAny(serviceCancellation.Task, source.Task)
                .ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            logger.LogInformation("Checking addresses...");

            var addresses = services
                .GetRequiredService<IServer>()
                ?.Features
                ?.Get<IServerAddressesFeature>()
                ?.Addresses
                ?.ToArray();

            if (addresses is null || addresses.Length == 0)
            {
                logger.LogError("Couldn't get addresses.");
                return;
            }

            logger.LogInformation("Addresses: {addresses}", string.Join(", ", addresses));

            var address = addresses
                .Where(v => v.StartsWith("http:"))
                .Select(v => v.Replace("[::]", "localhost"))
                .FirstOrDefault()
                ?? addresses.First();

            logger.LogInformation("Selected address: {address}", address);

            AppShellWindow.Start(address);
        }
    }
}
