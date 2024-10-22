using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Juniper.Data;

public static class WebApplicationBuilderExt
{
    public static WebApplicationBuilder AddJuniperDatabase<ContextT>(this WebApplicationBuilder builder, IDbProviderConfigurator configurator, string connectionString)
        where ContextT : DbContext
    {
        var isDev = builder.Environment.IsDevelopment();
        var collection = builder.Services.BuildServiceProvider().GetService<IDbProviderCollection>();
        if (collection is null)
        {
            builder.Services.AddSingleton(collection = new DbProviderCollection());
        }

        collection.Set<ContextT>(configurator);

        builder.Services.AddDbContext<ContextT>(options =>
        {
            configurator.ConfigureProvider(options, connectionString);
#if DEBUG
            if (!EF.IsDesignTime && isDev)
            {
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                options.EnableThreadSafetyChecks(true);
                options.LogTo(Console.WriteLine, (_, lvl) => LogLevel.Information <= lvl && lvl < LogLevel.Error);
                options.LogTo(Console.Error.WriteLine, LogLevel.Error);
            }
#endif
        });


        var identityInfo = Identity.FindIdentityContextType<ContextT>();

        builder.Services.AddSingleton((IDBContextInformation)new DbContextInformation(identityInfo is not null));

        if (identityInfo is not null)
        {
            var genericTypes = identityInfo.GetGenericArguments();
            var userType = genericTypes?[0] ?? typeof(IdentityUser);
            var roleType = genericTypes?[1] ?? typeof(IdentityRole);

            Action<IdentityOptions> setDefaultIdentityOptions = options =>
            {
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

            var addDefaultIdentityMethod = typeof(IdentityServiceCollectionUIExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == nameof(IdentityServiceCollectionUIExtensions.AddDefaultIdentity)
                    && m.GetParameters().Length == 2)
                .First()
                .MakeGenericMethod(userType);

            var identityBuilder = (IdentityBuilder)addDefaultIdentityMethod.Invoke(null, [builder.Services, setDefaultIdentityOptions])!;

            var addRolesMethod = identityBuilder
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == nameof(identityBuilder.AddRoles)
                    && m.GetParameters().Length == 0)
                .First()
                .MakeGenericMethod(roleType);

            addRolesMethod.Invoke(identityBuilder, []);

            identityBuilder.AddEntityFrameworkStores<ContextT>();
        }

        return builder;
    }
}