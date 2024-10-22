// Ignore Spelling: env Configurator

using System.Net.NetworkInformation;
using System.Reflection;
using Juniper.Configuration;
using Juniper.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Juniper;

public static class WebApplicationExt
{
    public static WebApplication UseJuniperRequestPipeline(this WebApplication app)
    {
        var env = app.Environment;
        var config = app.Configuration;
        var services = app.Services;

        var useWebSockets = config.GetValue<bool>("UseWebSockets");
        var httpsAddress = config.GetValue<string?>("Kestrel:Endpoints:HTTPS:Url");

        // Many of these operations are highly order-dependant. Don't change it
        // unless you can completely test all cases.

        app.UseHostFiltering();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/status/500");

            if (httpsAddress is not null)
            {
                app.UseHsts();
            }
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append(HeaderNames.XContentTypeOptions, "nosniff");
            await next();
        });

        if (httpsAddress is not null)
        {
            app.UseHttpsRedirection();
        }

        var defaultFileList = config.GetDefaultFiles();
        if (defaultFileList is not null)
        {
            var defaultFileOpts = new DefaultFilesOptions
            {
                DefaultFileNames = defaultFileList
            };

            app.UseDefaultFiles(defaultFileOpts);
        }

        var staticFileOpts = new StaticFileOptions
        {
            ContentTypeProvider = config.GetContentTypes(),
            OnPrepareResponse = (context) =>
            {
                if (!env.IsDevelopment())
                {
                    context.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={IConfigurationExt.CACHE_TIME}";
                }
            }
        };
        app.UseStaticFiles(staticFileOpts);

        var dbInfo = services.GetService<IDBContextInformation>();
        if (dbInfo?.UseIdentity == true)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        var logger = services.GetRequiredService<ILogger<StaticFileOptions>>();
        app.Use(async (context, next) =>
            {
                await next();

                if (context.Request.Path.Value?.StartsWith("/status/") != true
                    && context.Request.Accepts(MediaType.Text_Html)
                    && context.Response.StatusCode >= 400)
                {
                    try
                    {
                        context.Response.Redirect($"/status/{context.Response.StatusCode}?path={context.Request.Path}");
                    }
                    catch (Exception exp)
                    {
                        logger.LogError(exp, "Could not redirect to status page");
                    }
                }
            })
            .UseRouting();

        if (useWebSockets)
        {
            app.UseWebSockets();
        }

        app.MapControllers();
        app.MapRazorPages();

        if (app.Configuration.GetValue<bool>("UseSession"))
        {
            app.UseSession();
        }

        if (env.IsDevelopment())
        {
            app.UseDirectoryBrowser();
            app.UseHttpLogging();
        }

        return app;
    }

    public static WebApplication UseJuniperHub<HubT>(this WebApplication app)
        where HubT : Hub
    {
        var hubType = typeof(HubT);
        var hubPathAttr = hubType.GetCustomAttribute<HubPathAttribute>();
        var hubPath = hubPathAttr?.Path
            ?? throw new Exception("No SignalRHub path was not defined with a HubPathAttribute.");

        app.MapHub<HubT>(hubPath);
        return app;
    }

    public static WebApplication ShowNetworkInterfaces(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            var logger = app.Services.GetRequiredService<ILogger<NetworkInterface>>();

            foreach (var (Name, Address) in from i in NetworkInterface.GetAllNetworkInterfaces()
                                            where i.OperationalStatus == OperationalStatus.Up
                                               && i.NetworkInterfaceType != NetworkInterfaceType.Loopback
                                            let p = i.GetIPProperties()
                                            from a in p.UnicastAddresses
                                            where a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                            select (i.Name, a.Address))
            {
                logger.LogInformation("{Name}: {Address}", Name, Address);
            }
        }
        return app;
    }
}
