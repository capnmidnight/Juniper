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

    public static async Task ImportDataAsync<DbContextT>(this WebApplication app, string[] args, Dictionary<string, IDataImporter<DbContextT>> importers)
        where DbContextT : DbContext
    {
        var filesByType = new Dictionary<string, List<FileInfo>>();
        string? collecting = null;
        foreach (var arg in args)
        {
            if (collecting is null)
            {
                if (arg.StartsWith("--import-"))
                {
                    collecting = arg.Replace("--import-", "");
                    filesByType.Add(collecting, new List<FileInfo>());
                }
            }
            else if (arg.StartsWith("-"))
            {
                collecting = null;
            }
            else if (importers.ContainsKey(collecting))
            {
                var file = new FileInfo(arg);
                if (!file.Exists)
                {
                    Console.Error.WriteLine("File does not exist: {0}", file.FullName);
                }
                else
                {
                    filesByType[collecting].Add(file);
                }
            }
        }

        var keys = filesByType.Keys.ToArray();
        foreach (var key in keys)
        {
            if (filesByType[key].Count == 0)
            {
                filesByType.Remove(key);
            }
        }

        if (filesByType.Count > 0)
        {
            using var scope = app.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<DbContextT>();
            foreach (var (type, files) in filesByType)
            {
                if (importers.TryGetValue(type, out var importer))
                {
                    importer.Prepare(db);
                    foreach (var file in files)
                    {
                        importer.Import(file);
                    }
                }
            }
            await db.SaveChangesAsync();
        }
    }
}
