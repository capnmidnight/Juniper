using EntityFrameworkCore.Jet.Data;
using EntityFrameworkCore.Jet.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Juniper.Data;

public class Jet : AbstractDbProviderConfigurator<JetDbContextOptionsBuilder>
{
    public Jet(Action<JetDbContextOptionsBuilder>? optionsAction = null)
        : base(optionsAction)
    {

    }
    public Jet(string migrationAssemblyName)
        : this(builder => builder.MigrationsAssembly(migrationAssemblyName))
    {

    }

    public override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseJet(connectionString, DataAccessProviderType.OleDb, opts =>
        {
            OptionsAction?.Invoke(opts);
        });
}
