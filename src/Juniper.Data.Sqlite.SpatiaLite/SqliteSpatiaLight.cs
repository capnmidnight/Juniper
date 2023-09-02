using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Juniper.Data.SqlLite.SpatiaLite;

public class SqliteSpatiaLite : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(IConfiguration config, DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
            opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
