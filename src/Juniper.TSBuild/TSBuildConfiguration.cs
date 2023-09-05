using Juniper.TSBuild;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Juniper.Services;

public static class TSBuildConfiguration
{
    /// <summary>
    /// Adds an ESBuild-based TypeScript project compilation system to the currently running
    /// application. The compilers will run in a separate process, but report all progress
    /// back to this process.
    /// </summary>
    /// <typeparam name="BuildConfigT"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddJuniperBuildSystem<BuildConfigT>(this WebApplicationBuilder builder)
        where BuildConfigT : IBuildConfig, new()
    {
        builder.Services.AddJuniperBuildSystem<BuildConfigT>(builder.Environment);
        return builder;
    }

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
                .AddSingleton((Func<IServiceProvider, IBuildSystemService>)(serviceProvider =>
                    serviceProvider.GetRequiredService<BuildSystemService<BuildConfigT>>()))
                .AddHostedService(serviceProvider =>
                    serviceProvider.GetRequiredService<BuildSystemService<BuildConfigT>>());
        }
#endif

        return services;
    }

    /// <summary>
    /// A task that resolves when the ESBuild process has finished at least once.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task BuildAsync(this WebApplication app)
    {
        var buildService = app.Services.GetService<IBuildSystemService>();
        if (buildService is not null)
        {
            await buildService.Ready;
        }
    }

    /// <summary>
    /// Waits for the ESBuild process to finish at least once before starting the web server
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task BuildAndRunAsync(this WebApplication app)
    {
        await app.BuildAsync();
        await app.RunAsync();
    }
}