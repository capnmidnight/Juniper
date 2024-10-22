using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public interface IDbProviderConfigurator
{
    DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder, string connectionString);
}
