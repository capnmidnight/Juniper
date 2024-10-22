using Juniper;

var app = WebApplication
    .CreateBuilder(args)
    .AddJuniperBuildSystem<Juniper.Examples.BuildConfig>()
    .AddJuniperServices()
    .Build()
    .UseJuniperRequestPipeline();

await app.BuildAndRunAsync();