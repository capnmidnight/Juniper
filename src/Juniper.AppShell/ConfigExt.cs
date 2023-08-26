namespace Juniper.AppShell
{
    public static class ConfigExt
    {
        public static IServiceCollection AddAppShell(this IServiceCollection services)
        {
            return services.AddHostedService<AppShellService>();
        }

        public static WebApplication UseAppShell(this WebApplication app)
        {
            app.Urls.Add("http://[::]:0");
            return app;
        }
    }
}
