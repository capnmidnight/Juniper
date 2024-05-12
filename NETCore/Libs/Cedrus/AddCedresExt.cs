using System.Text.Json;
using System.Text.Json.Serialization;

using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;
using Juniper.Data;
using Juniper.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.Cedrus;
public static class AddCedresExt
{
    public static WebApplicationBuilder AddCedres<ProviderConfiguratorT>(this WebApplicationBuilder builder, ProviderConfiguratorT configurator, string connectionString)
        where ProviderConfiguratorT : class, IDbProviderConfigurator
    {
        builder.ConfigureJuniperDatabase(configurator)
            .WithConnectionString<CedrusContextInsecure>(connectionString)
            .AddPart<CedrusController>(options =>
            {
                options.AddJsonOptions(json =>
                {
                    json.JsonSerializerOptions.MaxDepth = 20;
                    json.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    json.JsonSerializerOptions.AllowTrailingCommas = true;
                    json.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    json.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    json.JsonSerializerOptions.WriteIndented = false;
                    json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            });

        builder.Services.AddScoped(services =>
            new CedrusContextSecure(
                services.GetRequiredService<CedrusContextInsecure>(),
                services.GetRequiredService<UserManager<CedrusUser>>(),
                services.GetRequiredService<RoleManager<CedrusRole>>()
            )
        );

        return builder;
    }

    public static WebApplication MigrateCedres(this WebApplication app) =>
        app
            .MigrateDatabase<CedrusContextInsecure>()
            .SeedData<CedrusContextInsecure>();
}
