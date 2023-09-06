using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public class NpgSql : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(connectionString, opts =>
            opts.EnableRetryOnFailure()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
