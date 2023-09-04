using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class IServiceProviderExtensions
{
    public static IServiceProvider MigrateDatabase<T>(this IServiceProvider services) where T : DbContext
    {
        using var scope = services.CreateScope();
        using var appContext = scope.ServiceProvider.GetRequiredService<T>();
        appContext.Database.AutoMigrate();
        return services;
    }

    public static void AutoMigrate(this DatabaseFacade database)
    {
        var pending = database.GetPendingMigrations();
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
            database.Migrate();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error during migration:\n{0}", ex.Unroll());
        }
    }

    public static IServiceCollection AddJuniperDatabase<ProviderConfiguratorT, ContextT>(this IServiceCollection services, string connectionString, bool detailedErrors)
        where ProviderConfiguratorT : IDbProviderConfigurator, new()
        where ContextT : DbContext
    {
        services.AddDbContext<ContextT>(options =>
            options.AddJuniperDatabase<ProviderConfiguratorT>(connectionString, detailedErrors));

        return services;
    }

    public static void AddJuniperDatabase<ProviderConfiguratorT>(this DbContextOptionsBuilder options, string connectionString, bool detailedErrors) 
        where ProviderConfiguratorT : IDbProviderConfigurator, new()
    {
        var providerConfigurator = new ProviderConfiguratorT();
        providerConfigurator.ConfigureProvider(options, connectionString);

        if (detailedErrors)
        {
            options.EnableDetailedErrors(true);
            options.EnableSensitiveDataLogging(true);
            options.EnableThreadSafetyChecks(true);
            options.LogTo(Console.WriteLine, (_, lvl) => LogLevel.Information <= lvl && lvl < LogLevel.Error);
            options.LogTo(Console.Error.WriteLine, LogLevel.Error);
        }
    }
}
