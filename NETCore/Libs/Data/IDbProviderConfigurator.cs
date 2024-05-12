using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public interface IDbProviderConfigurator
{
    void OnModelCreating(ModelBuilder modelBuilder);
    DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder, string connectionString);
}
