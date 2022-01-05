using Microsoft.EntityFrameworkCore;

namespace Juniper.EntityFramework
{
    public static class MigrationsManager
    {
        public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<T>();

            appContext.Database.Migrate();

            return host;
        }
    }
}
