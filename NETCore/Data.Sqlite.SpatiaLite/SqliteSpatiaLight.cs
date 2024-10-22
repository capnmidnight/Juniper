using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Juniper.Data;

public class SqliteSpatiaLite : Sqlite
{
    internal static Action<SqliteDbContextOptionsBuilder> UseNetTopologySuite(Action<SqliteDbContextOptionsBuilder>? optionsAction = null) =>
        opts =>
        {
            opts.UseNetTopologySuite();
            optionsAction?.Invoke(opts);
        };

    public SqliteSpatiaLite(Action<SqliteDbContextOptionsBuilder>? optionsAction, params SqlitePragma[] pragmas)
        : base(UseNetTopologySuite(optionsAction), pragmas)
    {
    }

    public SqliteSpatiaLite(params SqlitePragma[] pragmas)
        : base(UseNetTopologySuite(), pragmas)
    {
    }

    public SqliteSpatiaLite(string migrationsAssemblyName, params SqlitePragma[] pragmas)
        : base(UseNetTopologySuite(opts => opts.MigrationsAssembly(migrationsAssemblyName)), pragmas)
    { }
}
