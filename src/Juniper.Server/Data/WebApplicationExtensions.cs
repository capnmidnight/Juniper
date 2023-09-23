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

    public static async Task ImportDataAsync<DbContextT>(this WebApplication app, string[] args, Dictionary<string, IDataImporter<DbContextT>>? importers = null, IDataGenerator<DbContextT>? generator = null)
        where DbContextT : DbContext
    {
        using var scope = app.Services.CreateScope();
        if(generator is not null)
        {
            await scope.GenerateDataAsync(generator);
        }

        if (importers is not null)
        {
            await scope.ImportDataAsync(args, importers);
        }
    }
}
