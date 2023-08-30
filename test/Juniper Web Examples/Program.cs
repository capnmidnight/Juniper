using Juniper.Services;

using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureBuildSystem<Juniper.Examples.BuildConfig>()
    .ConfigureJuniperWebApplication();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpLogging(opts =>
    {
        opts.LoggingFields = HttpLoggingFields.All;
        opts.RequestHeaders.Add("host");
        opts.RequestHeaders.Add("user-agent");
    });
}

var app = builder
    .Build()
    .ConfigureJuniperRequestPipeline();

if (builder.Environment.IsDevelopment())
{
    app.UseHttpLogging();
}


await app.BuildReady();
await app.RunAsync();