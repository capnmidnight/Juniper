using Juniper.TSBuild;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Juniper;

public static class IServiceCollectionExt
{
    /// <summary>
    /// Adds an ESBuild-based TypeScript project compilation system to the currently running
    /// application. The compilers will run in a separate process, but report all progress
    /// back to this process.
    /// </summary>
    /// <typeparam name="BuildConfigT"></typeparam>
    /// <param name="services"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IServiceCollection AddJuniperBuildSystem<BuildConfigT>(this IServiceCollection services, IHostEnvironment environment)
        where BuildConfigT : IBuildConfig, new() =>
        services.AddJuniperBuildSystem<BuildConfigT>(environment.IsDevelopment());

    /// <summary>
    /// Adds an ESBuild-based TypeScript project compilation system to the currently running
    /// application. The compilers will run in a separate process, but report all progress
    /// back to this process.
    /// </summary>
    /// <typeparam name="BuildConfigT"></typeparam>
    /// <param name="services"></param>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public static IServiceCollection AddJuniperBuildSystem<BuildConfigT>(this IServiceCollection services, bool enabled)
        where BuildConfigT : IBuildConfig, new()
    {
#if DEBUG
        if (enabled)
        {
            services
                .AddSingleton<BuildSystemService<BuildConfigT>>()
                .AddSingleton<IBuildSystemService>(serviceProvider =>
                    serviceProvider.GetRequiredService<BuildSystemService<BuildConfigT>>());
        }
#endif

        return services;
    }
}
