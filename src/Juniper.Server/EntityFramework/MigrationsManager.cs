using Microsoft.EntityFrameworkCore;

namespace Juniper.EntityFramework
{
    public static class MigrationsManager
    {
        public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<T>();

            var pending = appContext.Database.GetPendingMigrations();
            if (pending.Any())
            {
                Console.WriteLine("Pending migrations: {0}", string.Join(", ", pending));
            }
            else
            {
                Console.WriteLine("No pending migrations");
            }

            try
            {
                appContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error during migration:\n{0}", ex.Unroll());
            }

            return host;
        }
    }
}
