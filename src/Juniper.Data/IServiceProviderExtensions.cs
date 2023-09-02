using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.Data;

public static class IServiceProviderExtensions
{
    public static IServiceProvider MigrateDatabase<T>(this IServiceProvider services) where T : DbContext
    {
        using var scope = services.CreateScope();
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

        return services;
    }
}
