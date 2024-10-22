using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Juniper;

public static class IHostApplicationBuilderExt
{

    public static HostBuilderT AddDebugHttpLogging<HostBuilderT>(this HostBuilderT builder)
        where HostBuilderT : IHostApplicationBuilder
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHttpLogging(opts =>
            {
                opts.LoggingFields = HttpLoggingFields.All;
                opts.RequestHeaders.Add("host");
                opts.RequestHeaders.Add("user-agent");
            });
        }

        return builder;
    }

    public static HostBuilderT AddPortDiscoveryService<HostBuilderT>(this HostBuilderT appBuilder)
        where HostBuilderT : IHostApplicationBuilder
    {
        appBuilder.Services.AddPortDiscoveryService();
        return appBuilder;
    }
}
