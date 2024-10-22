using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Juniper.Data;

public record SqlitePragma(string Key, string Value);

public class Sqlite : AbstractDbProviderConfigurator<SqliteDbContextOptionsBuilder>
{
    private readonly SqlitePragma[] pragmas;

    public Sqlite(Action<SqliteDbContextOptionsBuilder>? optionsAction, params SqlitePragma[] pragmas)
        : base(optionsAction)
    {
        this.pragmas = pragmas;
    }

    public Sqlite(params SqlitePragma[] pragmas)
        : this((Action<SqliteDbContextOptionsBuilder>?)null, pragmas)
    {
    }

    public Sqlite(string migrationsAssemblyName, params SqlitePragma[] pragmas)
        : this(builder => builder.MigrationsAssembly(migrationsAssemblyName), pragmas)
    { }


    public static void CleanupTempFiles(DirectoryInfo dir)
    {
        if (dir.Exists)
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
    }

    public override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

    public override void ConfigureContext(DbContext context)
    {
        base.ConfigureContext(context);
        var connection = context.Database.GetDbConnection();
        connection.StateChange += (object sender, StateChangeEventArgs e) =>
        {
            if (e.OriginalState != ConnectionState.Open && e.CurrentState == ConnectionState.Open)
            {
                var query = string.Join("\n", pragmas.Select(pragma =>
                    string.Format("PRAGMA {0} = {1};\n", pragma.Key, pragma.Value)
                ));
                context.Database.ExecuteSqlRaw(query);
            }
        };
    }

    public override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
        {
            opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            OptionsAction?.Invoke(opts);
        });
}
