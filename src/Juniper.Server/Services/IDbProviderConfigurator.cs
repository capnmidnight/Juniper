using Microsoft.EntityFrameworkCore;

namespace Juniper;

public interface IDbProviderConfigurator
{
    DbContextOptionsBuilder ConfigureProvider(IWebHostEnvironment env, IConfiguration config, DbContextOptionsBuilder options, string connectionString);
}
