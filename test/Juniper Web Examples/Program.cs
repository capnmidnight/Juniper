using Juniper.Services;

var app = WebApplication
    .CreateBuilder(args)
    .ConfigureBuildSystem<Juniper.Examples.BuildConfig>()
    .ConfigureJuniperWebApplication()
    .Build()
    .ConfigureJuniperRequestPipeline();

await app.BuildReady();
await app.RunAsync();