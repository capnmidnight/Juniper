using Juniper.TSBuild;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Juniper.Services;

public interface IPortDiscoveryService
{
    Task<Uri> GettingAddress { get; }
}

/// <summary>
/// A <see cref="BackgroundService"/> that discovers the port the application
/// is running on, which is useful when binding to port 0 (random port selection).
/// </summary>
public class PortDiscoveryService : BackgroundService, IPortDiscoveryService
{
    private readonly TaskCompletionSource appStarting = new();
    private readonly TaskCompletionSource appStopping = new();
    private readonly CancellationTokenSource serviceCanceller = new();
    private readonly TaskCompletionSource<Uri> addressFetching = new();

    public Task<Uri> GettingAddress => addressFetching.Task;

    private readonly IServiceProvider services;
    private readonly ILogger logger;
    private readonly IBuildSystemService? buildSystem;

    public PortDiscoveryService(IServiceProvider services, IHostApplicationLifetime lifetime, ILogger<PortDiscoveryService> logger)
    {
        this.services = services;
        this.logger = logger;
        buildSystem = services.GetService<IBuildSystemService>();

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

            if (buildSystem is not null)
            {
                await buildSystem.Started;
            }

            addressFetching.TrySetResult(address);
            logger.LogInformation("running at address {address}", address);
            Console.WriteLine("running at address {0}", address);
            logger.LogInformation("listening on port {port}", address.Port);
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
    }
}
