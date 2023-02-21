namespace Juniper.Configuration
{
    public static partial class IConfigurationExt
    {
        public static string? GetAzureSubscriptionKey(this IConfiguration config) =>
            config.GetValue<string>("Azure:Speech:SubscriptionKey");

        public static string? GetAzureRegion(this IConfiguration config) =>
            config.GetValue<string>("Azure:Speech:Region");

        public static string? GetOpenAIKey(this IConfiguration config) =>
            config.GetValue<string>("OpenAI:APIKey");
    }
}
