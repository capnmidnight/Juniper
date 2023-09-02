using OpenAI.Extensions;

namespace Juniper.OpenAI;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureJuniperOpenAI(this WebApplicationBuilder builder, IConfiguration config)
    {
        var openAIKey = config?.GetValue<string>("APIKey");
        if (openAIKey is not null)
        {
            builder.Services.AddOpenAIService(settings =>
                settings.ApiKey = openAIKey);
        }
        return builder;
    }
}