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
        var logger = services.GetRequiredService<ILogger<T>>();
        appContext.Database.AutoMigrate(logger);
        return services;
    }

    public static void AutoMigrate(this DatabaseFacade database, ILogger? logger = null)
    {
        var pending = database.GetPendingMigrations();
        if (pending.Any())
        {
            logger?.LogInformation("Pending migrations: {migrations}", string.Join(", ", pending));
        }
        else
        {
            logger?.LogInformation("No pending migrations");
        }

        try
        {
            database.Migrate();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error during migration");
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
