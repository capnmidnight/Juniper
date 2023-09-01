using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Juniper.Server.SqlLite.SpatiaLite;

public class SqliteSpatiaLite : IDbProviderConfigurator
{
    public DbContextOptionsBuilder ConfigureProvider(IWebHostEnvironment env, IConfiguration config, DbContextOptionsBuilder options, string connectionString) =>
        options.UseSqlite(connectionString);
}
