using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public class SqliteSpatiaLite : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
        {
            opts.UseNetTopologySuite()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        });
}
