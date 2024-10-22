using Juniper;
using Juniper.AppShell;
using Juniper.Cedrus;
using Juniper.Data;

using Microsoft.EntityFrameworkCore;

#if WINDOWS
using AppShellFactory = Juniper.AppShell.WpfAppShellFactory;
#else
using AppShellFactory = Juniper.AppShell.AvaloniaAppShellFactory;
#endif

if (args.Contains("--build"))
{
    await Juniper.TSBuild.BuildSystem<Juniper.Cedrus.Example.BuildConfig>.Run(args);
}
else
{
    var builder = WebApplication.CreateBuilder(args);
    var seedDB = builder.Configuration["Import"] == "seed";

    builder.AddCedrus(
        new DefaultSqlite(opts => opts.MigrationsAssembly("Juniper.Cedrus.Example.Migrations.Sqlite")),
        "name=ConnectionStrings:Sqlite"
    );

    if (EF.IsDesignTime)
    {
        await builder
            .Build()
            .RunAsync();
    }
    else if(seedDB)
    {
        await builder
            .Build()
            .MigrateCedrus(seedDB)
            .RunAsync();
    }
    else 
    {
        await builder
            .AddJuniperBuildSystem<Juniper.Cedrus.Example.BuildConfig>()
            .AddJuniperServices()
            .AddJuniperMenuHider()
            .AddJuniperAppShell<AppShellFactory>()
            .Build()
            .MigrateCedrus(seedDB)
            .UseJuniperRequestPipeline()
            .BuildAndRunAsync();
    }
}