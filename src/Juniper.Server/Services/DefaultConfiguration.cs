using Juniper.Configuration;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Juniper.Services
{
    public static class DefaultConfiguration
    {
        private const string DEFAULT_ADMIN_PATH = "/Admin";

        public class Options
        {
            public string AdminPath { get; set; } = DEFAULT_ADMIN_PATH;
            public bool UseIdentity { get; set; } = true;
            public bool UseEmail { get; set; } = true;
            public bool UseSignalR { get; set; } = true;
        }

        public static void ConfigureDatabase<ContextT>(this IServiceCollection services, IWebHostEnvironment env, string connectionStringName)
            where ContextT : DbContext
        {
            services.AddDbContext<ContextT>(options =>
            {
                options.UseNpgsql($"name=ConnectionStrings:{connectionStringName}", opts =>
                    opts.EnableRetryOnFailure()
                        .UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

                if (env.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });
        }

        public static void ConfigureAuthentication<ContextT>(this IServiceCollection services)
            where ContextT : IdentityDbContext
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

        public static void ConfigureSignalR(this IServiceCollection services)
        {
            services.AddSignalR(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
                options.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });
        }

        public static void ConfigureViews(this IServiceCollection services, IWebHostEnvironment env, string adminPath = DEFAULT_ADMIN_PATH)
        {
            services.AddControllersWithViews();

            var razorPages = services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder(adminPath);
            });

            if (env.IsDevelopment())
            {
                razorPages.AddRazorRuntimeCompilation();
            }
        }

        public static void ConfigureLogging(this IServiceCollection services, IWebHostEnvironment env, bool useSignalR = true)
        {
            if (env.IsDevelopment())
            {
                services.AddLogging(options =>
                {
                    var console = options.AddConsole();

                    console.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

                    if (useSignalR)
                    {
                        console.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                    }
                });
            }
        }

        public static IServiceCollection ConfigureDefaultServices<ContextT>(this IServiceCollection services, IWebHostEnvironment env, string connectionStringName, Options options = null)
            where ContextT : IdentityDbContext
        {
            options ??= new();

            services.AddTransient<IConfigureOptions<KestrelServerOptions>, LetsEncryptService>();
            services.ConfigureDatabase<ContextT>(env, connectionStringName);

            if (options.UseIdentity)
            {
                services.ConfigureAuthentication<ContextT>();
            }

            if (options.UseEmail)
            {
                services.AddTransient<IEmailSender, EmailSender>();
            }

            if (options.UseSignalR)
            {
                services.ConfigureSignalR();
            }

            services.ConfigureViews(env, options.AdminPath);
            services.ConfigureLogging(env, options.UseSignalR);

            return services;
        }

        private static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, Action<IEndpointRouteBuilder> configEndPoint)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            })
                .UseHttpsRedirection()
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
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseWebSockets()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapRazorPages();
                    if (configEndPoint is not null)
                    {
                        configEndPoint(endpoints);
                    }
                });

            return app;
        }


        public static IApplicationBuilder ConfigureRequestPipeline(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            return app.ConfigureRequestPipeline(env, config, null);
        }


        public static IApplicationBuilder ConfigureRequestPipeline<HubT>(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config, string hubPath)
            where HubT : Hub
        {
            return app.ConfigureRequestPipeline(env, config, endpoints =>
            {
                endpoints.MapHub<HubT>(hubPath);
            });
        }
    }
}
