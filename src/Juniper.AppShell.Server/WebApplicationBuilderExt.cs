using Juniper.Services;

namespace Juniper.AppShell
{
    public static class WebApplicationBuilderExt
    {
        public static WebApplication AddAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder)
            where AppShellWindowFactoryT : IAppShellFactory, new()
        {
            appBuilder.WebHost.ConfigureJuniperWebHost();

            appBuilder.Services
                .ConfigureDefaultServices(appBuilder.Environment)
                .UseAppShell<AppShellWindowFactoryT>();

            var app = appBuilder.Build();
            var logger = app.Services.GetRequiredService<ILogger<AppShellService<AppShellWindowFactoryT>>>();

            app.ConfigureRequestPipeline(appBuilder.Environment, app.Configuration, logger);

            return app;
        }

        /// <summary>
        /// Configures the WebHost to run on localhost with a random port and registers a service that opens
        /// a window constructed by the provided <typeparamref name="AppShellWindowFactoryT"/> factory.
        /// </summary>
        /// <typeparam name="AppShellWindowFactoryT">A concrete instance of the <see cref="IAppShellFactory"/> interface </typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="useHttps">Defaults to false</param>
        /// <returns><paramref name="hostBuilder"/></returns>
        public static IServiceCollection UseAppShell<AppShellWindowFactoryT>(this IServiceCollection services)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
                services
                    // Give DI the class it needs to create
                    .AddSingleton<AppShellService<AppShellWindowFactoryT>>()
                    // Give DI an alias that other DI consumers can use to request the service without
                    // knowing the specific type of `AppShellWindowFactorT`
                    .AddSingleton<IAppShell>((serviceProvider) =>
                        serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
                    // Register the instance as a service so it will run.
                    .AddHostedService((serviceProvider) =>
                        serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>());
    }
}
