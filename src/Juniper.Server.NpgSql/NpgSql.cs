using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Juniper.Server.NpgSql;

public class NpgSql : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(IWebHostEnvironment env, IConfiguration config, DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(connectionString, opts =>
            opts.EnableRetryOnFailure()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
}
