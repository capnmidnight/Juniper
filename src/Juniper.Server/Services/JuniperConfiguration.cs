// Ignore Spelling: env Configurator

using Juniper.Configuration;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using System.Reflection;

namespace Juniper.Services
{

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

        public static WebApplicationBuilder ConfigureJuniperWebApplication(this WebApplicationBuilder builder)
        {
            var webBuilder = builder.WebHost;
            var config = builder.Configuration;
            var services = builder.Services;
            var env = builder.Environment;
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Environment: {0}:{1}", envName, BUILD);

#if DEBUG
            if (!env.IsDevelopment())
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly is not null)
                {
                    builder.Configuration.AddUserSecrets(assembly);
                }
            }
#endif

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

            if (!env.IsDevelopment())
            {
                services.AddTransient<IConfigureOptions<KestrelServerOptions>, LetsEncryptService>();
            }

            services.Configure<KestrelServerOptions>(options =>
                options.AllowSynchronousIO = false);

            services.AddControllersWithViews();

            var razorPages = services.AddRazorPages(options =>
            {
                var adminPath = config.GetValue<string?>("AdminPath");
                if (!string.IsNullOrEmpty(adminPath))
                {
                    options.Conventions.AuthorizeFolder(adminPath);
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

            if (config.GetValue<object>("Mail") is not null)
            {
                services.AddTransient<IEmailSender, EmailSender>();
            }

            if (!string.IsNullOrEmpty(config.GetValue<string>("SignalRHub")))
            {
                services.AddSignalR(options =>
                {
                    options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
                    options.HandshakeTimeout = TimeSpan.FromSeconds(5);
                });
            }

            if (env.IsDevelopment())
            {
                services.AddHttpLogging(opts =>
                {
                    opts.LoggingFields = HttpLoggingFields.All;
                    opts.RequestHeaders.Add("host");
                    opts.RequestHeaders.Add("user-agent");
                });
            }

            return builder;
        }

        public static WebApplicationBuilder ConfigureJuniperDatabase<ProviderConfiguratorT, ContextT>(this WebApplicationBuilder builder, string connectionStringName)
            where ProviderConfiguratorT : IDbProviderConfigurator, new()
            where ContextT : IdentityDbContext
        {
            var env = builder.Environment;
            var config = builder.Configuration;
            var services = builder.Services;

            services.AddDbContext<ContextT>(options =>
            {
                var providerConfigurator = new ProviderConfiguratorT();
                providerConfigurator.ConfigureProvider(env, config, options, $"name=ConnectionStrings:{connectionStringName}");

                var detailedErrors = config.GetValue<bool>("DetailedErrors");
                if (env.IsDevelopment()
                    && detailedErrors)
                {
                    options.EnableDetailedErrors(true);
                    options.EnableSensitiveDataLogging(true);
                    options.EnableThreadSafetyChecks(true);
                    options.LogTo(Console.WriteLine, (_, lvl) => LogLevel.Information <= lvl && lvl < LogLevel.Error);
                    options.LogTo(Console.Error.WriteLine, LogLevel.Error);
                }
            });

            var useIdentity = config.GetValue<bool>("UseIdentity");
            if (useIdentity)
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

            return builder;
        }
       
        public static WebApplication ConfigureJuniperRequestPipeline(this WebApplication app)
        {
            var env = app.Environment;
            var config = app.Configuration;

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

            var logger = app.Services.GetRequiredService<ILogger<StaticFileOptions>>();
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

            var useIdentity = config.GetValue<bool>("UseIdentity");
            if (useIdentity)
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

            if (env.IsDevelopment())
            {
                app.UseHttpLogging();
            }

            return app;
        }

        public static WebApplication ConfigureJuniperHub<HubT>(this WebApplication app)
            where HubT : Hub
        {
            var hubPath = app.Configuration.GetValue<string?>("SignalRHub")
                ?? throw new Exception("No SignalRHub path defined in configuration.");

            app.MapHub<HubT>(hubPath); ;
            return app;
        }

    }
}