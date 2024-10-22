using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Juniper.Data;

public class DefaultNpgSql : NpgSql
{
    public DefaultNpgSql(Action<NpgsqlDbContextOptionsBuilder>? optionsAction = null)
        : base(optionsAction)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DefaultNpgSql(string migrationsAssemblyName)
        : this(builder => builder.MigrationsAssembly(migrationsAssemblyName))
    { }
}