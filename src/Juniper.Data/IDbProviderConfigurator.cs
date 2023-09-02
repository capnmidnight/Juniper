using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Juniper.Data;

public interface IDbProviderConfigurator
{
    DbContextOptionsBuilder ConfigureProvider(IConfiguration config, DbContextOptionsBuilder options, string connectionString);
}
