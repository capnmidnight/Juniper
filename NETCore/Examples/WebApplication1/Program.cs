using Juniper.AppShell;
using Juniper.Cedrus;
using Juniper.Data;
using Juniper.Services;

await WebApplication
    .CreateBuilder(args)
    .ConfigureJuniperWebApplication()
    .AddCedres(new Sqlite(), "name=ConnectionStrings:Cedrus")
    .AddJuniperBuildSystem<WebApplication1.BuildConfig>()
    .ConfigureJuniperAppShell<WpfAppShellFactory>()
    .Build()
    .MigrateCedres()
    .ConfigureJuniperRequestPipeline()
    .BuildAndRunAsync();