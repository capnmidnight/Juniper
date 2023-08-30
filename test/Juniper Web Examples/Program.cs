using Juniper.Examples;
using Juniper.Services;
using Juniper.TSBuild;

using Microsoft.AspNetCore.HttpLogging;


BuildSystem<BuildConfig>? build;

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureJuniperWebApplication();

if (builder.Environment.IsDevelopment())
{
    try
    {
        build = new BuildSystem<BuildConfig>();
        await build.WatchAsync();
    }
    catch (BuildSystemProjectRootNotFoundException exp)
    {
        Console.WriteLine("WARNING: {0}", exp.Message);
    }

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

await app.RunAsync();