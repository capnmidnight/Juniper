using Juniper;
using Juniper.Configuration;
using Juniper.Converters;
using Juniper.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;
using System.Text.Json.Serialization;

namespace Juniper;


/// <summary>
/// Extension methods that configure defaults for how I like
/// to setup my servers.
/// </summary>
public static class WebApplicationBuilderExt
{

#if DEBUG
    private const string BUILD = "DEBUG";
#else
    private const string BUILD = "RELEASE";
#endif

    public static WebApplicationBuilder AddSystemd(this WebApplicationBuilder builder)
    {
        builder.Services.AddSystemd();
        return builder;
    }

    public static WebApplicationBuilder AddPart<T>(this WebApplicationBuilder builder, Action<IMvcBuilder>? mvcBuilderAction = null)
    {
        var mvc = builder.Services.AddControllers().AddApplicationPart(typeof(T).Assembly);
        mvcBuilderAction?.Invoke(mvc);
        return builder;
    }

    public static WebApplicationBuilder AddJuniperServices(this WebApplicationBuilder builder, Action<CookieAuthenticationOptions>? configureCookieAuth = null) =>
        builder.AddJuniperServices(null, configureCookieAuth);

    public static WebApplicationBuilder AddJuniperServices(this WebApplicationBuilder builder, Action<RazorPagesOptions>? configureRazorPages, Action<CookieAuthenticationOptions>? configureCookieAuth = null)
    {
        Console.WriteLine("Environment: {0}:{1}", builder.Environment.EnvironmentName, BUILD);

        builder.Services.Configure<KestrelServerOptions>(options =>
            options.AllowSynchronousIO = false);

        return builder
            .AddDebugAssemblyUserSecrets()
            .AddLetsEncrypt()
            .AddAuthentication(configureCookieAuth)
            .AddRoutes(configureRazorPages)
            .AddSession()
            .AddEmail()
            .AddHubs()
            .AddDebugHttpLogging();
    }

    private static WebApplicationBuilder AddLetsEncrypt(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment()
                    && builder.Configuration.GetValue<string?>("Kestrel:Endpoints:HTTPS:Url") is not null)
        {
            builder.Services.AddTransient<IConfigureOptions<KestrelServerOptions>, LetsEncryptService>();
        }

        return builder;
    }

    private static WebApplicationBuilder AddDebugAssemblyUserSecrets(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddAssemblyUserSecrets(builder.Environment);
        return builder;
    }

    private static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder, Action<CookieAuthenticationOptions>? configureCookieAuth)
    {
        builder.Services.Configure<IISOptions>(options =>
        {
            options.ForwardClientCertificate = false;
            options.AutomaticAuthentication = true;
        });

        if (builder.Configuration.GetValue<bool>("UseWindowsAuth"))
        {
            builder.Services
                .AddAuthorization(options =>
                {
                    options.FallbackPolicy = options.DefaultPolicy;
                })
                .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();
        }

        if (builder.Configuration.GetValue<bool>("UseCookies"))
        {
            builder.Services.Configure<CookieAuthenticationOptions>(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;

                options.ExpireTimeSpan = TimeSpan.FromDays(5);
                options.SlidingExpiration = true;

                configureCookieAuth?.Invoke(options);
            });
        }

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
            opts.JsonSerializerOptions.Converters.Add(new ListJsonConverter());
        });


        if (builder.Environment.IsDevelopment())
        {
            razorPages.AddRazorRuntimeCompilation();

            builder.Services
                .AddDirectoryBrowser()
                .AddLogging(options =>
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

    public static WebApplicationBuilder AddHubs(this WebApplicationBuilder builder, bool force = false)
    {
        if (force || HasAnyHubs)
        {
            builder.Services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
                options.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });
        }

        return builder;
    }

    private static bool HasAnyHubs
    {
        get
        {
            var curAssembly = Assembly.GetEntryAssembly();
            var types = curAssembly?.GetTypes();
            var hubPathAttrs = types
                ?.Select(t => t.GetCustomAttribute<HubPathAttribute>())
                ?.Where(a => a is not null)
                ?.ToArray();
            return hubPathAttrs?.Length > 0;
        }
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

    /// <summary>
    /// Registers a service that allows the left-side menu to be collapsed as soon as the page opens.
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <returns><paramref name="appBuilder"/></returns>
    public static WebApplicationBuilder AddJuniperMenuHider(this WebApplicationBuilder appBuilder) =>
        appBuilder.AddPart<MenuHiderController>();

    public static WebApplicationBuilder UseSystemd(this WebApplicationBuilder builder)
    {
        builder.Services.AddSystemd();
        return builder;
    }
}
