using Juniper.Configuration;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using System.Reflection;

namespace Juniper.Services
{

    /// <summary>
    /// Extension methods that configure defaults for how I like
    /// to setup my servers.
    /// </summary>
    public static class DefaultConfiguration
    {
        private const string DEFAULT_ADMIN_PATH = "/Admin";

        public class BasicOptions
        {
            public string AdminPath { get; set; } = DEFAULT_ADMIN_PATH;
            public bool UseEmail { get; set; } = true;
            public bool UseSignalR { get; set; } = true;
        }

        public static IServiceCollection ConfigureDefaultServices(this IServiceCollection services, IWebHostEnvironment env, BasicOptions? config = null)
        {
            config ??= new();

            if (!env.IsDevelopment())
            {
                services.AddTransient<IConfigureOptions<KestrelServerOptions>, LetsEncryptService>();
            }

            services.Configure<KestrelServerOptions>(options =>
                options.AllowSynchronousIO = false);

            services.AddControllersWithViews();

            var razorPages = services.AddRazorPages(options =>
            {
                if (!string.IsNullOrEmpty(config.AdminPath))
                {
                    options.Conventions.AuthorizeFolder(config.AdminPath);
                }
            });

            if (env.IsDevelopment())
            {
                razorPages.AddRazorRuntimeCompilation();

                services.AddLogging(options =>
                    options.AddConsole()
                        .AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug)
                );
            }

            if (config.UseEmail)
            {
                services.AddTransient<IEmailSender, EmailSender>();
            }

            if (config.UseSignalR)
            {
                services.AddSignalR(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(5);
                });
            }

            return services;
        }

        public class Options : BasicOptions
        {
            public bool UseIdentity { get; set; } = true;
            public bool LogSQL { get; set; }
        }

        public static DbContextOptionsBuilder SetDefaultConnection(this DbContextOptionsBuilder options, string connectionStringName, IWebHostEnvironment? env = null, Options? config = null)
        {
            options.UseNpgsql($"name=ConnectionStrings:{connectionStringName}", opts =>
                opts.EnableRetryOnFailure()
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

            if (env?.IsDevelopment() == true
                && config?.LogSQL == true)
            {
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                options.EnableThreadSafetyChecks(true);
                options.LogTo(Console.WriteLine, (_, lvl) => LogLevel.Information <= lvl && lvl < LogLevel.Error);
                options.LogTo(Console.Error.WriteLine, LogLevel.Error);
            }

            return options;
        }

        public static IServiceCollection ConfigureDefaultServices<ContextT>(this IServiceCollection services, IWebHostEnvironment env, string connectionStringName, Options? config = null)
            where ContextT : IdentityDbContext
        {
            config ??= new();

            services.ConfigureDefaultServices(env, config);

            services.AddDbContext<ContextT>(options =>
                options.SetDefaultConnection(connectionStringName, env, config));


            if (config.UseIdentity)
            {
                services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    var isDev = env.IsDevelopment();
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequireDigit = !isDev;
                    options.Password.RequireLowercase = !isDev;
                    options.Password.RequireUppercase = !isDev;
                    options.Password.RequireNonAlphanumeric = !isDev;
                    options.Password.RequiredLength = isDev ? 0 : 8;
                    options.Password.RequiredUniqueChars = 1;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.AllowedForNewUsers = true;

                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<ContextT>();

                services.Configure<CookieAuthenticationOptions>(options =>
                {
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                    options.Cookie.HttpOnly = true;

                    options.ExpireTimeSpan = TimeSpan.FromDays(5);
                    options.SlidingExpiration = true;

                    options.LoginPath = "/Identity/Account/Login";
                    options.LogoutPath = "/Identity/Account/Logout";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });
            }

            return services;
        }

        private static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, ILogger logger, bool withAuth, Action<IEndpointRouteBuilder>? configEndPoint)
        {
            var useWebSockets = config.GetValue<bool>("UseWebSockets");
            var httpsPort = config.GetValue<int?>("Ports:HTTPS");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                if (httpsPort is not null)
                {
                    app.UseHsts();
                }
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add(HeaderNames.XContentTypeOptions, "nosniff");
                await next();
            });

            if (httpsPort is not null)
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

            app.UseStaticFiles(staticFileOpts)
                .Use(async (context, next) =>
                {
                    await next();

                    if (context.Request.Path.Value?.StartsWith("/status/") != true
                        && !context.Request.Accepts(MediaType.Application_Json)
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

            if (withAuth)
            {
                app.UseAuthentication()
                    .UseAuthorization();
            }

            if (useWebSockets)
            {
                app.UseWebSockets();
            }

            return app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapRazorPages();
                    if (configEndPoint is not null)
                    {
                        configEndPoint(endpoints);
                    }
                });
        }


        public static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, ILogger logger)
        {
            return app.ConfigureRequestPipeline(env, config, logger, false, null);
        }


        public static IApplicationBuilder ConfigureRequestPipeline<HubT>(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, ILogger logger, string hubPath)
            where HubT : Hub
        {
            return app.ConfigureRequestPipeline(env, config, logger, true, endpoints =>
                endpoints.MapHub<HubT>(hubPath));
        }

#if DEBUG
        private const string BUILD = "DEBUG";
#else
        private const string BUILD = "RELEASE";
#endif

        public static IHostBuilder ConfigureJuniperHost<StartupT>(this IHostBuilder host)
            where StartupT : class
        {
            return host.UseSystemd()
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.ConfigureJuniperWebHost<StartupT>());
        }

        public static IWebHostBuilder ConfigureJuniperWebHost<StartupT>(this IWebHostBuilder webBuilder)
            where StartupT : class
        {
            webBuilder.UseStartup<StartupT>();
            return ConfigureJuniperWebHost(webBuilder);
        }

        public static IWebHostBuilder ConfigureJuniperWebHost(this IWebHostBuilder webBuilder)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Environment: {0}:{1}", env, BUILD);

#if DEBUG
            if (env != Environments.Development)
            {
                webBuilder.ConfigureAppConfiguration(configBuilder =>
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (assembly is not null)
                    {
                        configBuilder.AddUserSecrets(assembly);
                    }
                });
            }
#endif

            return webBuilder.ConfigureAppConfiguration(configBuilder =>
            {
                var config = configBuilder.Build();
                var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
                var httpPort = config.GetValue<uint?>("Ports:HTTP");
                var httpsPort = config.GetValue<uint?>("Ports:HTTPS");
                var urls = new List<string>();
                if (httpPort is not null
                    && (httpPort != 80
                        || isWindows))
                {
                    urls.Add($"http://127.0.0.1:{httpPort}");
                }

                if (httpsPort is not null
                    && (httpPort != 443
                        || isWindows))
                {
                    urls.Add($"https://127.0.0.1:{httpsPort}");
                }

                if (urls.Count > 0)
                {
                    webBuilder.UseUrls(urls.ToArray());
                }
            });
        }
    }
}
