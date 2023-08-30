// Ignore Spelling: Configurator
using Juniper.AppShell;

namespace Juniper.Services
{
    public static class JuniperAppShellConfiguration
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
                .AddSingleton<IAppShell>((serviceProvider) =>
                    serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
                // Register the instance as a service so it will run.
                .AddHostedService((serviceProvider) =>
                    serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>());
            return appBuilder;
        }
    }
}
