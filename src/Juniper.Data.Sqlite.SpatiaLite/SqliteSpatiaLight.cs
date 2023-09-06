using Microsoft.EntityFrameworkCore;

namespace Juniper.Data.SqlLite;

public class SqliteSpatiaLite : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
            opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
