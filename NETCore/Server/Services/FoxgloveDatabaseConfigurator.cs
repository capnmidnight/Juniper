// Ignore Spelling: env Configurator

using System.Reflection;

using Juniper.Data;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Juniper.Services;

public class JuniperDatabaseConfigurator<ProviderConfiguratorT>
        where ProviderConfiguratorT : class, IDbProviderConfigurator
{
    private readonly WebApplicationBuilder builder;
    private readonly ProviderConfiguratorT configurator;

    public JuniperDatabaseConfigurator(WebApplicationBuilder builder, ProviderConfiguratorT configurator)
    {
        this.builder = builder;
        this.configurator = configurator;
    }

    public WebApplicationBuilder WithConnectionString<ContextT>(string connectionString)
        where ContextT : DbContext
    {
        var env = builder.Environment;
        var config = builder.Configuration;
        var services = builder.Services;

        services.AddJuniperDatabase<ProviderConfiguratorT, ContextT>(configurator, connectionString, env.IsDevelopment());

        var testType = typeof(ContextT);
        var identityInfo = typeof(IdentityDbContext)
            .Assembly
            .GetTypes()
            .Select(t =>
            {
                var here = testType;
                while (here is not null)
                {
                    if (here == t || (here.IsGenericType && here.GetGenericTypeDefinition() == t))
                    {
                        return here;
                    }

                    here = here.BaseType;
                }

                return null;
            })
            .Where(t => t is not null)
            .FirstOrDefault();

        services.AddSingleton<IDBContextInformation>(new DbContextInformation(identityInfo is not null));

        if (identityInfo is not null)
        {
            var genericTypes = identityInfo.GetGenericArguments();
            var userType = genericTypes?[0] ?? typeof(IdentityUser);
            var roleType = genericTypes?[1] ?? typeof(IdentityRole);

            var identExtType = typeof(IdentityServiceCollectionUIExtensions);
            var addDefaultIdentityMethod = identExtType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == nameof(IdentityServiceCollectionUIExtensions.AddDefaultIdentity)
                    && m.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(userType);
            Action<IdentityOptions> setDefaultIdentityOptions = options =>
            {
                var isDev = env.IsDevelopment();
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = !isDev;
                options.Password.RequireLowercase = !isDev;
                options.Password.RequireUppercase = !isDev;
                options.Password.RequireNonAlphanumeric = !isDev;
                options.Password.RequiredLength = isDev ? 0 : 12;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
            };

            var identityBuilder = (IdentityBuilder)addDefaultIdentityMethod.Invoke(null, new object[] { services, setDefaultIdentityOptions })!;
            var identityBuilderType = identityBuilder.GetType();
            var addRolesMethod = identityBuilderType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == nameof(identityBuilder.AddRoles)
                    && m.GetParameters().Length == 0)
                .First()
                .MakeGenericMethod(roleType);

            identityBuilder = (IdentityBuilder)addRolesMethod.Invoke(identityBuilder, Array.Empty<object>())!;

            identityBuilder.AddEntityFrameworkStores<ContextT>();

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

            if (config.GetValue<bool>("UseWindowsAuth"))
            {
                services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                    .AddNegotiate();

                services.AddAuthorization(options =>
                {
                    options.FallbackPolicy = options.DefaultPolicy;
                });
            }
        }

        return builder;
    }
}