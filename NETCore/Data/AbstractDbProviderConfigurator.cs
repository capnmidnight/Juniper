using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;
public abstract class AbstractDbProviderConfigurator<T> : IDbProviderConfigurator
{
    protected Action<T>? OptionsAction { get; }

    protected AbstractDbProviderConfigurator(Action<T>? optionsAction)
    {
        OptionsAction = optionsAction;
    }

    public virtual void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    public virtual void ConfigureContext(DbContext context)
    {
    }

    public abstract DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder optionsBuilder, string connectionString);
}
