// Ignore Spelling: Configurator
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Juniper.AppShell;

public static class AppShellConfiguration
{

    /// <summary>
    /// Registers a service that opens a window constructed by the provided <typeparamref name="AppShellWindowFactoryT"/> factory.
    /// </summary>
    /// <typeparam name="AppShellWindowFactoryT">A concrete instance of the <see cref="IAppShellFactory"/> interface </typeparam>
    /// <param name="appBuilder"></param>
    /// <returns><paramref name="appBuilder"/></returns>
    public static WebApplicationBuilder ConfigureJuniperAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder)
        where AppShellWindowFactoryT : IAppShellFactory, new()
    {
        appBuilder.Services
            // Give DI the class it needs to create
            .AddSingleton<AppShellService<AppShellWindowFactoryT>>()
            // Give DI an alias that other DI consumers can use to request the service without
            // knowing the specific type of `AppShellWindowFactorT`
            .AddSingleton<IAppShellService>((serviceProvider) =>
                serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
            // Give DI an alias that other DI consumers can use to request the app shell without
            // knowing the specific type of `AppShellWindowFactorT`
            .AddSingleton<IAppShell>((serviceProvider) =>
                serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>());
        return appBuilder;
    }

    public static async Task StartAppShellAsync(this WebApplication app, string firstPage)
    {
        await app.StartAsync();
        var service = app.Services.GetRequiredService<IAppShellService>();
        await service.StartAsync(firstPage);
    }

    public static async Task RunAppShellAsync(this WebApplication app)
    {
        var service = app.Services.GetRequiredService<IAppShellService>();
        var shell = await service.RunAsync();
        await shell.WaitForCloseAsync();
        await app.StopAsync();
        await app.WaitForShutdownAsync();
    }
}
