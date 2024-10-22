using Microsoft.EntityFrameworkCore;

using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Juniper.Data;

public class NpgSql : AbstractDbProviderConfigurator<NpgsqlDbContextOptionsBuilder>
{
    public NpgSql(Action<NpgsqlDbContextOptionsBuilder>? optionsAction = null)
        : base(optionsAction)
    {

    }

    public NpgSql(string migrationAssemblyName)
        : this(builder => builder.MigrationsAssembly(migrationAssemblyName))
    { }

    public override DbContextOptionsBuilder ConfigureProvider(DbContextOptionsBuilder options, string connectionString) =>
        options.UseNpgsql(connectionString, opts =>
        {
            opts.EnableRetryOnFailure()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
            OptionsAction?.Invoke(opts);
        });
}
