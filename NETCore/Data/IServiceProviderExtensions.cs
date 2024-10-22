using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class IServiceProviderExtensions
{
    public static IServiceProvider MigrateDatabase<DbContextT>(this IServiceProvider services)
        where DbContextT : DbContext
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContextT>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbContextT>>();
        db.Database.AutoMigrate(logger);
        return services;
    }

    public static IServiceProvider SeedData<DbContextT>(this IServiceProvider services, IConfiguration config)
        where DbContextT : DbContext
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContextT>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbContextT>>();
        db.ApplySeedersAsync(scope.ServiceProvider, config, logger).Wait();
        return services;
    }
}
