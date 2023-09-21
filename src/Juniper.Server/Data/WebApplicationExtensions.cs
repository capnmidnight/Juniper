using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.Data;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDatabase<T>(this WebApplication app) where T : DbContext
    {
        app.Services.MigrateDatabase<T>();
        return app;
    }

    public static Task ImportDataAsync<DbContextT>(this WebApplication app, string[] args, Dictionary<string, IDataImporter<DbContextT>> importers)
        where DbContextT : DbContext
    {
        using var scope = app.Services.CreateScope();
        return scope.ImportDataAsync(args, importers);
    }
}
