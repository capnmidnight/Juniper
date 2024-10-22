using System.Text.Json;
using System.Text.Json.Serialization;
using Juniper.Cedrus.Controllers.V1;
using Juniper.Cedrus.Entities;
using Juniper.Cedrus.Models;
using Juniper.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Juniper.Cedrus;

public static class WebApplicationBuilderExt
{
    public static WebApplicationBuilder AddCedrus(this WebApplicationBuilder builder, IDbProviderConfigurator configurator, string connectionString)
    {
        builder.AddJuniperDatabase<CedrusContextInsecure>(configurator, connectionString)
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
}
