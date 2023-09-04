using Microsoft.EntityFrameworkCore;

namespace Juniper.Data.SqlLite.SpatiaLite;

public class SqliteSpatiaLite : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
            opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
