namespace Juniper.AppShell
{
    public static class ConfigExt
    {
        public static IServiceCollection AddAppShell<AppShellWindowFactoryT>(this IServiceCollection services)
            where AppShellWindowFactoryT : IAppShellFactory, new() =>
            services
                .AddSingleton<AppShellService<AppShellWindowFactoryT>>()
                .AddSingleton<IAppShell>((serviceProvider) =>
                    serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>())
                .AddHostedService((serviceProvider) =>
                    serviceProvider.GetRequiredService<AppShellService<AppShellWindowFactoryT>>());

        public static WebApplication UseAppShell(this WebApplication app)
        {
            app.Urls.Add("http://[::]:0");
            return app;
        }
    }
}
