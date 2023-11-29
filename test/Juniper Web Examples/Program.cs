using Juniper.Services;

var app = WebApplication
    .CreateBuilder(args)
    .AddJuniperBuildSystem<Juniper.Examples.BuildConfig>()
    .ConfigureJuniperWebApplication()
    .Build()
    .ConfigureJuniperRequestPipeline();

await app.BuildAndRunAsync();