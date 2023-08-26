namespace Juniper.AppShell
{
    public static class ConfigExt
    {
        public static IServiceCollection AddAppShell<AppShellWindowFactoryT>(this IServiceCollection services)
            where AppShellWindowFactoryT : IAppShellFactory, new()
        {
            return services.AddHostedService<AppShellService<AppShellWindowFactoryT>>();
        }

        public static WebApplication UseAppShell(this WebApplication app)
        {
            app.Urls.Add("http://[::]:0");
            return app;
        }
    }
}
