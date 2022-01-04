using Microsoft.Extensions.Configuration;

namespace Juniper.Configuration
{
    public static partial class IConfigurationExt
    {
        public static string GetAzureSubscriptionKey(this IConfiguration config)
        {
            return config.GetSection("Azure")
                .GetValue<string>("SubscriptionKey");
        }

        public static string GetAzureRegion(this IConfiguration config)
        {
            return config.GetSection("Azure")
                .GetValue<string>("Region");
        }
    }
}
