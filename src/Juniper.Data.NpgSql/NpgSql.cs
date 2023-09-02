using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Juniper.Data.NpgSql;

public class NpgSql : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(IConfiguration config, DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(connectionString, opts =>
            opts.EnableRetryOnFailure()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
