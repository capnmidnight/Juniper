using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using OpenAI.Extensions;

namespace Juniper.OpenAI;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureJuniperOpenAI(this WebApplicationBuilder builder)
    {
        var apiKey = builder.Configuration.GetValue<string>("OpenAI:APIKey");
        if (apiKey is not null)
        {
            builder.Services.AddOpenAIService(settings =>
                settings.ApiKey = apiKey);
        }
        return builder;
    }
}