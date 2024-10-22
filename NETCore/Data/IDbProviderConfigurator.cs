using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public interface IDbProviderConfigurator
{
    void OnModelCreating(ModelBuilder modelBuilder);
    void ConfigureContext(DbContext context);
    DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder, string connectionString);
}
