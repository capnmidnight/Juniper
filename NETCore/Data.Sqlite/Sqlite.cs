using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Data;

public class Sqlite : AbstractDbProviderConfigurator<SqliteDbContextOptionsBuilder>
{
    public Sqlite(Action<SqliteDbContextOptionsBuilder>? optionsAction = null)
        : base(optionsAction)
    {

    }

    public static void CleanupTempFiles(DirectoryInfo dir)
    {
        var tempFiles = dir
            .GetFiles()
            .Where(file => file.Extension == ".db-shm");

        foreach (var tempFile in tempFiles)
        {
            try
            {
                tempFile.Delete();
            }
            catch
            {
                Console.Error.WriteLine("Couldn't delete temp-file {0}", tempFile.Name);
            }
        }
    }

    public override void OnModelCreating(ModelBuilder builder)
    {
        // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
        // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
        // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
        // use the DateTimeOffsetToBinaryConverter
        // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
        // This only supports millisecond precision, but should be sufficient for most use cases.
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var properties = from p in entityType.ClrType.GetProperties()
                             where p.PropertyType == typeof(DateTimeOffset)
                                || p.PropertyType == typeof(DateTimeOffset?)
                             select p;
            foreach (var property in properties)
            {
                builder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(new DateTimeOffsetToBinaryConverter());
            }
        }
    }

    public override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
        {
            opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            OptionsAction?.Invoke(opts);
        });
}
