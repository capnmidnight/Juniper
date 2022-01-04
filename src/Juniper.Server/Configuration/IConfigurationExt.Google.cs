using Microsoft.Extensions.Configuration;

namespace Juniper.Configuration
{
    public static partial class IConfigurationExt
    {
        public static string GetGoogleAPIKey(this IConfiguration config)
        {
            return config.GetSection("Google")
                .GetValue<string>("APIKey");
        }

        public static string GetGoogleSigningKey(this IConfiguration config)
        {
            return config.GetSection("Google")
                .GetValue<string>("SigningKey");
        }
    }
}
