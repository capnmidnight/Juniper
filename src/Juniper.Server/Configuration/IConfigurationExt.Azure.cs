namespace Juniper.Configuration
{
    public static partial class IConfigurationExt
    {
        public static string? GetAzureSubscriptionKey(this IConfiguration config) =>
            config.GetSection("Azure")
                .GetSection("Speech")
                .GetValue<string>("SubscriptionKey");

        public static string? GetAzureRegion(this IConfiguration config) =>
            config.GetSection("Azure")
                .GetSection("Speech")
                .GetValue<string>("Region");

        public static string? GetOpenAIKey(this IConfiguration config) =>
            config.GetSection("OpenAI")
                .GetValue<string>("APIKey");
    }
}
