using Juniper.Services;

using Microsoft.AspNetCore.SignalR;

namespace Juniper.AppShell
{
    public static class JuniperAppShellConfiguration
    {
        public static IHostBuilder ConfigureJuniperHostAppShell<AppShellWindowFactoryT, StartupT>(this IHostBuilder host)
            where AppShellWindowFactoryT : IAppShellFactory, new()
            where StartupT : class =>
            host.ConfigureJuniperHost<StartupT>()
                .ConfigureServices(services =>
                    services.UseAppShell<AppShellWindowFactoryT>());

        public static IWebHostBuilder ConfigureJuniperWebHostAppShell<AppShellWindowFactoryT, StartupT>(this IWebHostBuilder webHost)
            where AppShellWindowFactoryT : IAppShellFactory, new()
            where StartupT : class =>
            webHost.ConfigureJuniperWebHost<StartupT>()
                .ConfigureServices(services =>
                    services.UseAppShell<AppShellWindowFactoryT>());

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder, Action<IServiceCollection> configureServices, Action<WebApplication, IWebHostEnvironment, IConfiguration, ILogger> configureRequestPipeline)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
            appBuilder.ConfigureJuniperWebApplication(
                configureServices.Join(services =>
                    services.UseAppShell<AppShellWindowFactoryT>()),
                configureRequestPipeline);

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder, Action<IServiceCollection> configureServices)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
            appBuilder.ConfigureJuniperWebApplication(
                configureServices.Join(services =>
                    services.UseAppShell<AppShellWindowFactoryT>()));

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder, Action<WebApplication, IWebHostEnvironment, IConfiguration, ILogger> configureRequestPipeline)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
            appBuilder.ConfigureJuniperWebApplication(services =>
                services.UseAppShell<AppShellWindowFactoryT>(),
                configureRequestPipeline);

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT>(this WebApplicationBuilder appBuilder)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
            appBuilder.ConfigureJuniperWebApplication(services =>
                services.UseAppShell<AppShellWindowFactoryT>());

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT, HubT>(this WebApplicationBuilder appBuilder, Action<IServiceCollection> configureServices, Action<WebApplication, IWebHostEnvironment, IConfiguration, ILogger> configureRequestPipeline)
            where AppShellWindowFactoryT : IAppShellFactory, new()
            where HubT : Hub =>
            appBuilder.ConfigureJuniperWebApplication<HubT>(
                configureServices.Join(services =>
                    services.UseAppShell<AppShellWindowFactoryT>()),
                configureRequestPipeline);

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT, HubT>(this WebApplicationBuilder appBuilder, Action<IServiceCollection> configureServices)
            where AppShellWindowFactoryT : IAppShellFactory, new()
            where HubT : Hub =>
            appBuilder.ConfigureJuniperWebApplication<HubT>(
                configureServices.Join(services =>
                    services.UseAppShell<AppShellWindowFactoryT>()));

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT, HubT>(this WebApplicationBuilder appBuilder, Action<WebApplication, IWebHostEnvironment, IConfiguration, ILogger> configureRequestPipeline)
            where AppShellWindowFactoryT : IAppShellFactory, new()
            where HubT : Hub =>
            appBuilder.ConfigureJuniperWebApplication<HubT>(services =>
                services.UseAppShell<AppShellWindowFactoryT>(),
                configureRequestPipeline);

        public static WebApplication ConfigureJuniperWebAppShell<AppShellWindowFactoryT, HubT>(this WebApplicationBuilder appBuilder)
            where AppShellWindowFactoryT : IAppShellFactory, new()
            where HubT : Hub =>
            appBuilder.ConfigureJuniperWebApplication<HubT>(services =>
                services.UseAppShell<AppShellWindowFactoryT>());

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
