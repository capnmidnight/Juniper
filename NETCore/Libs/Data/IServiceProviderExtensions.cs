using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        db.ApplySeeders(scope.ServiceProvider, config, logger);
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

    public static IServiceCollection AddJuniperDatabase<ProviderConfiguratorT, ContextT>(this IServiceCollection services, ProviderConfiguratorT providerConfigurator, string connectionString, bool detailedErrors)
        where ProviderConfiguratorT : class, IDbProviderConfigurator
        where ContextT : DbContext
    {
        var collection = services.BuildServiceProvider().GetService<IDbProviderCollection>();
        if(collection is null)
        {
            services.AddSingleton(collection = new DbProviderCollection());
        }

        collection.Set<ContextT>(providerConfigurator);

        services.AddDbContext<ContextT>(options =>
        {
            providerConfigurator.ConfigureProvider(options, connectionString);
#if DEBUG
            if (!EF.IsDesignTime && detailedErrors)
            {
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                options.EnableThreadSafetyChecks(true);
                options.LogTo(Console.WriteLine, (_, lvl) => LogLevel.Information <= lvl && lvl < LogLevel.Error);
                options.LogTo(Console.Error.WriteLine, LogLevel.Error);
            }
#endif
        });

        return services;
    }

    private static ILogger CreateLogger(IServiceScope scope, Type type)
    {
        var createLoggerMethod = typeof(IServiceProviderExtensions)
            .GetMethod(nameof(CreateLogger), BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new Exception("Can't find createLoggerMethod");

        var createLogger = createLoggerMethod.MakeGenericMethod(type);
        var logger = createLogger.Invoke(null, new[] { scope }) as ILogger
            ?? throw new Exception("Couldn't create logger");

        return logger;
    }

    private static ILogger<T> CreateLogger<T>(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<ILogger<T>>();
    }

    public static async Task GenerateDataAsync<DbContextT>(this IServiceScope scope, IDataGenerator<DbContextT> generator)
        where DbContextT : DbContext
    {
        using var db = scope.ServiceProvider.GetRequiredService<DbContextT>();

        var logger = CreateLogger(scope, generator.GetType());

        generator.Import(scope.ServiceProvider, db, logger);

        await db.SaveChangesAsync();
    }

    public static Task ImportDataAsync<DbContextT>(this IServiceScope scope, string[] args, Dictionary<string, IDataImporter<DbContextT>> importers)
        where DbContextT : DbContext
    {
        using var db = scope.ServiceProvider.GetRequiredService<DbContextT>();
        var createLoggerMethod = typeof(IServiceProviderExtensions)
            .GetMethod(nameof(CreateLogger), BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new Exception("Can't find createLoggerMethod");

        ILogger createLogger(IDataImporter<DbContextT> importer)
        {
            var type = importer.GetType();
            var createLogger = createLoggerMethod.MakeGenericMethod(type);
            var logger = createLogger.Invoke(null, new[] { scope }) as ILogger
                ?? throw new Exception("Couldn't create logger");
            return logger;
        }

        return db.ImportDataAsync(args, importers, createLogger);
    }

    public static async Task ImportDataAsync<DbContextT>(this DbContextT db, string[] args, Dictionary<string, IDataImporter<DbContextT>> importers, Func<IDataImporter<DbContextT>, ILogger> createLogger)
        where DbContextT : DbContext
    {
        string? collecting = null;
        foreach (var arg in args)
        {
            if (arg.StartsWith("-"))
            {
                collecting = null;

                if (arg.StartsWith("--import-"))
                {
                    collecting = arg.Replace("--import-", "");
                    if (!importers.ContainsKey(collecting))
                    {
                        Console.Error.WriteLine($"Don't know how to import {collecting.ToUpperInvariant()} files.");
                        collecting = null;
                    }
                }
                else if (arg != "--import")
                {
                    Console.Error.WriteLine($"Invalid switch/argument! {arg}");
                    return;
                }
            }
            else
            {
                var file = new FileInfo(arg);
                if (!file.Exists)
                {
                    Console.Error.WriteLine("File does not exist: {0}", file.FullName);
                    return;
                }

                var type = collecting ?? file.Extension[1..];

                if (importers.TryGetValue(type, out var importer))
                {
                    var logger = createLogger(importer);
                    logger.LogInformation("Importing {fileName}... ", file.FullName);
                    importer.Import(db, file, logger);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
