using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OpenAI.Extensions;

namespace Juniper.OpenAI;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection AddJuniperOpenAI(this IServiceCollection services, IConfiguration config)
    {
        var openAIKey = config?.GetValue<string>("APIKey");
        if (openAIKey is not null)
        {
            services.AddOpenAIService(settings =>
                settings.ApiKey = openAIKey);
        }
        return services;
    }

    public static WebApplicationBuilder AddJuniperOpenAI(this WebApplicationBuilder builder, string configGroup)
    {
        builder.Services.AddJuniperOpenAI(builder.Configuration.GetSection(configGroup));
        return builder;
    }
}