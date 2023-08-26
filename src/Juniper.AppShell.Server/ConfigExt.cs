namespace Juniper.AppShell
{
    public static class ConfigExt
    {
        /// <summary>
        /// Configures the WebHost to run on localhost with a random port and registers a service that opens
        /// a window constructed by the provided <typeparamref name="AppShellWindowFactoryT"/> factory.
        /// </summary>
        /// <typeparam name="AppShellWindowFactoryT">A concrete instance of the <see cref="IAppShellFactory"/> interface </typeparam>
        /// <param name="hostBuilder"></param>
        /// <param name="useHttps">Defaults to false</param>
        /// <returns><paramref name="hostBuilder"/></returns>
        public static IWebHostBuilder UseAppShell<AppShellWindowFactoryT>(this IWebHostBuilder hostBuilder, bool useHttps = false)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
            (useHttps 
                // Random port selection so multiple instances of the app can run.
                // Explicitly set 127.0.0.1 so a network share prompt doesn't appear.
                ? hostBuilder.UseUrls("https://127.0.0.1:0", "http://127.0.0.1:0")
                : hostBuilder.UseUrls("http://127.0.0.1:0")
            ).ConfigureServices(services =>
                services
                    // Give DI the class it needs to create
                    .AddSingleton<AppShellService<AppShellWindowFactoryT>>()
                    // Give DI an alias that other DI consumers can use to request the service without
                    // knowing the specific type of `AppShellWindowFactorT`
                    .AddSingleton<IAppShell>((serviceProvider) =>
                        serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
                    // Register the instance as a service so it will run.
                    .AddHostedService((serviceProvider) =>
                        serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>()));
    }
}
