using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.Data;

public class DefaultSqliteSpatiaLight : DefaultSqlite
{

    public DefaultSqliteSpatiaLight(params SqlitePragma[] pragmas)
        : this(SqliteSpatiaLite.UseNetTopologySuite(), pragmas) { }

    public DefaultSqliteSpatiaLight(Action<SqliteDbContextOptionsBuilder>? optionsAction, params SqlitePragma[] pragmas)
        : base(SqliteSpatiaLite.UseNetTopologySuite(optionsAction), new[]
        {
            new SqlitePragma("journal_mode", "WAL"),
            new SqlitePragma("busy_timout", "5000"),
            new SqlitePragma("synchronous", "NORMAL"),
            new SqlitePragma("cache_size", "-20000"),
            new SqlitePragma("foreign_keys", "true"),
            new SqlitePragma("temp_store", "memory")
        }.Union(pragmas).ToArray())
    {

    }

    public DefaultSqliteSpatiaLight(string migrationAssemblyName, params SqlitePragma[] pragmas)
        : this(SqliteSpatiaLite.UseNetTopologySuite(builder => builder.MigrationsAssembly(migrationAssemblyName)), pragmas)
    { }
}