using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.Data;

public class DefaultSqlite : Sqlite
{
    public DefaultSqlite(params SqlitePragma[] pragmas)
        : this((Action<SqliteDbContextOptionsBuilder>?)null, pragmas) { }

    public DefaultSqlite(Action<SqliteDbContextOptionsBuilder>? optionsAction, params SqlitePragma[] pragmas)
        : base(optionsAction, [
            new SqlitePragma("journal_mode", "WAL"),
            new SqlitePragma("busy_timout", "5000"),
            new SqlitePragma("synchronous", "NORMAL"),
            new SqlitePragma("cache_size", "-20000"),
            new SqlitePragma("foreign_keys", "true"),
            new SqlitePragma("temp_store", "memory"),
            ..pragmas
        ])
    {

    }

    public DefaultSqlite(string migrationAssemblyName, params SqlitePragma[] pragmas)
        : this(builder => builder.MigrationsAssembly(migrationAssemblyName), pragmas)
    { }

#if DEBUG
    private static bool cleanupAttempted = false;
    public override void ConfigureContext(DbContext context)
    {
        if (!cleanupAttempted)
        {
            cleanupAttempted = true;
            var connection = context.Database.GetDbConnection();
            var dbFile = new FileInfo(connection.DataSource);
            if (dbFile.Directory is not null)
            {
                CleanupTempFiles(dbFile.Directory);
            }
        }
        base.ConfigureContext(context);
    }
#endif
}