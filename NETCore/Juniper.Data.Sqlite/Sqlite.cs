using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public class Sqlite : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString, opts =>
            opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
