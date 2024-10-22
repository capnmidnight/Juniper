// Ignore Spelling: env Configurator

using Juniper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper;

public static class IServerCollectionExt 
{
    public static IServiceCollection AddPortDiscoveryService(this IServiceCollection services) =>
        services
            .AddSingleton<PortDiscoveryService>()
            .AddSingleton<IPortDiscoveryService>(serviceProvider =>
                serviceProvider.GetRequiredService<PortDiscoveryService>())
            .AddHostedService(serviceProvider =>
                serviceProvider.GetRequiredService<PortDiscoveryService>());
}
