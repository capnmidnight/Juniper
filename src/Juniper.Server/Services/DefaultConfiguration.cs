using Juniper.Configuration;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

        public static IServiceCollection ConfigureDefaultServices(this IServiceCollection services, IWebHostEnvironment env, BasicOptions config = null)
        {
            config ??= new();

            if (!env.IsDevelopment())
            {
                services.AddTransient<IConfigureOptions<KestrelServerOptions>, LetsEncryptService>();
            }

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

                    if (config.UseSignalR)
                    {
                        console.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                    }
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

        public static IServiceCollection ConfigureDefaultServices<ContextT>(this IServiceCollection services, IWebHostEnvironment env, string connectionStringName, Options config = null)
            where ContextT : IdentityDbContext
        {
            services.ConfigureDefaultServices(env, config);

            services.AddDbContext<ContextT>(options =>
            {
                options.UseNpgsql($"name=ConnectionStrings:{connectionStringName}", opts =>
                    opts.EnableRetryOnFailure()
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

                if (env.IsDevelopment() && config.LogSQL)
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

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
                    options.Cookie.SameSite = SameSiteMode.Strict;
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

        private static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, bool withAuth, bool withWebSockets, Action<IEndpointRouteBuilder> configEndPoint)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error")
                    .UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            })
                .UseHttpsRedirection()
                .UseStatusCodePagesWithRedirects("/status/{0}")
                .UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = config.GetContentTypes(),
                    OnPrepareResponse = (context) =>
                    {
                        if (!env.IsDevelopment())
                        {
                            context.Context.Response.Headers["Cache-Control"] = $"public,max-age={IConfigurationExt.CACHE_TIME}";
                        }
                    }
                })
                .UseDefaultFiles(new DefaultFilesOptions
                {
                    DefaultFileNames = config.GetDefaultFiles()
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


        public static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            return app.ConfigureRequestPipeline(env, config, false, false, null);
        }


        public static IApplicationBuilder ConfigureRequestPipeline<HubT>(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, string hubPath)
            where HubT : Hub
        {
            return app.ConfigureRequestPipeline(env, config, true, false, endpoints =>
            {
                endpoints.MapHub<HubT>(hubPath);
            });
        }

        public struct PortOptions
        {
            public int HttpPort { get; set; }
            public int HttpsPort { get; set; }
        }

        public static IHostBuilder ConfigureJuniperHost<StartupT>(this IHostBuilder host, PortOptions? ports = null)
            where StartupT : class
        {
            return host.UseSystemd()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupT>();
#if DEBUG
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != Environments.Development)
                    {
                        webBuilder.ConfigureAppConfiguration(app =>
                        {
                            app.AddUserSecrets(Assembly.GetEntryAssembly());
                        });
                    }

                    if (ports is not null && Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        webBuilder.UseUrls(
                            $"https://*:{ports.Value.HttpsPort}",
                            $"http://*:{ports.Value.HttpPort}");
                    }
#endif
                });
        }
    }
}
