// Ignore Spelling: env Configurator

using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json.Serialization;

using Juniper.Configuration;
using Juniper.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Juniper.Services;


/// <summary>
/// Extension methods that configure defaults for how I like
/// to setup my servers.
/// </summary>
public static class JuniperConfiguration
{

#if DEBUG
    private const string BUILD = "DEBUG";
#else
    private const string BUILD = "RELEASE";
#endif

    public static IConfigurationBuilder AddAssemblyUserSecrets(this IConfigurationBuilder configBuilder, IHostEnvironment env)
    {
#if DEBUG
        if (!env.IsDevelopment())
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is not null)
            {
                configBuilder.AddUserSecrets(assembly);
            }
        }
#endif
        return configBuilder;
    }

    public static WebApplicationBuilder UseSystemd(this WebApplicationBuilder builder)
    {
        builder.Services.AddSystemd();
        return builder;
    }

    public static IWebHostBuilder UseSystemd(this IWebHostBuilder builder) =>
        builder.ConfigureServices(services => services.AddSystemd());

    public static WebApplicationBuilder AddPart<T>(this WebApplicationBuilder builder, Action<IMvcBuilder>? mvcBuilderAction = null)
    {
        var mvc = builder.Services.AddControllers().AddApplicationPart(typeof(T).Assembly);
        mvcBuilderAction?.Invoke(mvc);
        return builder;
    }

    public static WebApplicationBuilder ConfigureJuniperWebApplication(this WebApplicationBuilder builder, Action<RazorPagesOptions>? configureRazorPages = null)
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Console.WriteLine("Environment: {0}:{1}", envName, BUILD);

        builder.Configuration.AddAssemblyUserSecrets(builder.Environment);

        if (!builder.Environment.IsDevelopment()
            && builder.Configuration.GetValue<string?>("Kestrel:Endpoints:HTTPS:Url") is not null)
        {
            builder.Services.AddTransient<IConfigureOptions<KestrelServerOptions>, LetsEncryptService>();
        }

        builder.Services.Configure<KestrelServerOptions>(options =>
            options.AllowSynchronousIO = false);

        builder.Services
            .AddSingleton<PortDiscoveryService>()
            .AddSingleton<IPortDiscoveryService>(serviceProvider =>
                serviceProvider.GetRequiredService<PortDiscoveryService>())
            .AddHostedService(serviceProvider =>
                serviceProvider.GetRequiredService<PortDiscoveryService>());


        builder
            .AddRoutes(configureRazorPages)
            .AddSession()
            .AddEmail()
            .AddHubs()
            .AddLogging();

        return builder;
    }

    private static WebApplicationBuilder AddRoutes(this WebApplicationBuilder builder, Action<RazorPagesOptions>? configureRazorPages = null)
    {
        builder.Services.AddControllersWithViews(opts =>
            opts.InputFormatters.Add(new TextPlainInputFormatter()));

        var razorPages = builder.Services.AddRazorPages(configureRazorPages);

        razorPages.AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            opts.JsonSerializerOptions.MaxDepth = 10;
        });

        if (builder.Environment.IsDevelopment())
        {
            razorPages.AddRazorRuntimeCompilation();

            builder.Services.AddDirectoryBrowser();

            builder.Services.AddLogging(options =>
                options.AddConsole()
                    .AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug)
            );
        }

        return builder;
    }

    private static WebApplicationBuilder AddSession(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<bool>("UseSession"))
        {
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });
        }

        return builder;
    }

    private static WebApplicationBuilder AddEmail(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<object>("Mail") is not null)
        {
            builder.Services.AddTransient<IEmailSender, EmailSender>();
        }

        return builder;
    }

    private static WebApplicationBuilder AddHubs(this WebApplicationBuilder builder)
    {
        var curAssembly = Assembly.GetEntryAssembly();
        var types = curAssembly?.GetTypes();
        var hubPathAttrs = types
            ?.Select(t => t.GetCustomAttribute<HubPathAttribute>())
            ?.Where(a => a is not null)
            ?.ToArray();

        if (hubPathAttrs?.Length > 0)
        {
            builder.Services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
                options.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });
        }

        return builder;
    }

    private static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
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

    public static WebApplication ConfigureJuniperRequestPipeline(this WebApplication app)
    {
        var env = app.Environment;
        var config = app.Configuration;
        var services = app.Services;

        var useWebSockets = config.GetValue<bool>("UseWebSockets");
        var httpsAddress = config.GetValue<string?>("Kestrel:Endpoints:HTTPS:Url");

        app.UseHostFiltering();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");

            if (httpsAddress is not null)
            {
                app.UseHsts();
            }
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add(HeaderNames.XContentTypeOptions, "nosniff");
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

        var logger = services.GetRequiredService<ILogger<StaticFileOptions>>();
        app.UseStaticFiles(staticFileOpts)
            .Use(async (context, next) =>
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

        var dbInfo = services.GetService<IDBContextInformation>();
        if (dbInfo?.UseIdentity == true)
        {
            app.UseAuthentication()
                .UseAuthorization();
        }

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
    

    public static JuniperDatabaseConfigurator<ProviderConfiguratorT> ConfigureJuniperDatabase<ProviderConfiguratorT>(this WebApplicationBuilder builder, ProviderConfiguratorT configurator)
        where ProviderConfiguratorT : class, IDbProviderConfigurator
    {
        return new JuniperDatabaseConfigurator<ProviderConfiguratorT>(builder, configurator);
    }

    public static WebApplicationBuilder AddHttpClient(this WebApplicationBuilder builder, string name)
    {
        builder.Services.AddHttpClient(name, client =>
        {
            client.DefaultRequestHeaders.Add("sec-ch-ua", @"Chromium"";v=""122"", ""Not(A:Brand"";v=""24"", ""Microsoft Edge"";v=""122""");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "Windows");
            client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
            client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
            client.DefaultRequestHeaders.Add("sec-fetch-site", "none");
            client.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
            client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.Add("user-agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36 Edg/122.0.0.0");
        });

        return builder;
    }

    public static WebApplication ConfigureJuniperHub<HubT>(this WebApplication app)
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
