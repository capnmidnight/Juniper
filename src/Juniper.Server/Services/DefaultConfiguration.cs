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
            {
                options.AllowSynchronousIO = false;
            });

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
                {
                    var console = options.AddConsole();

                    console.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

                    //if (config.UseSignalR)
                    //{
                    //    console.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                    //}
                });
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
                    options.User.RequireUniqueEmail = true;

                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
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

        private static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, PortOptions? ports, bool withAuth, bool withWebSockets, Action<IEndpointRouteBuilder>? configEndPoint)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                if (ports?.HttpsPort > 0)
                {
                    app.UseHsts();
                }
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add(HeaderNames.XContentTypeOptions, "nosniff");
                await next();
            });

            if (ports?.HttpsPort > 0)
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
                        && context.Response.StatusCode >= 400)
                    {
                        context.Response.Redirect($"/status/{context.Response.StatusCode}?path={context.Request.Path}");
                    }
                })
                .UseRouting();

            if (withAuth)
            {
                app.UseAuthentication()
                    .UseAuthorization();
            }

            if (withWebSockets)
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


        public static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, PortOptions? ports)
        {
            return app.ConfigureRequestPipeline(env, config, ports, false, false, null);
        }


        public static IApplicationBuilder ConfigureRequestPipeline<HubT>(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, PortOptions? ports, string hubPath)
            where HubT : Hub
        {
            return app.ConfigureRequestPipeline(env, config, ports, true, false, endpoints =>
                endpoints.MapHub<HubT>(hubPath));
        }

        public struct PortOptions
        {
            public uint? HttpPort { get; set; }
            public uint? HttpsPort { get; set; }
        }

#if DEBUG
        private const string BUILD = "DEBUG";
#else
        private const string BUILD = "RELEASE";
#endif

        public static IHostBuilder ConfigureJuniperHost<StartupT>(this IHostBuilder host, PortOptions? ports = null)
            where StartupT : class
        {
            return host.UseSystemd()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    Console.WriteLine("Environment: {0}:{1}", env, BUILD);

                    webBuilder.UseStartup<StartupT>();
#if DEBUG
                    if (env != Environments.Development)
                    {
                        webBuilder.ConfigureAppConfiguration(app => {
                            var assembly = Assembly.GetEntryAssembly();
                            if (assembly is not null)
                            {
                                app.AddUserSecrets(assembly);
                            }
                        });
                    }

                    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        var urls = new List<string>();
                        if (ports?.HttpPort > 0)
                        {
                            urls.Add($"http://*:{ports.Value.HttpPort}");
                        }

                        if (ports?.HttpsPort > 0)
                        {
                            urls.Add($"https://*:{ports.Value.HttpsPort}");
                        }

                        if (urls.Count > 0)
                        {
                            webBuilder.UseUrls(urls.ToArray());
                        }
                    }
#endif
                });
        }
    }
}
