namespace Juniper.Configuration
{
    public static partial class IConfigurationExt
    {
        public static string? GetGoogleAPIKey(this IConfiguration config) =>
            config.GetSection("Google").GetValue<string>("APIKey");

        public static string? GetGoogleSigningKey(this IConfiguration config) =>
            config.GetSection("Google")
                .GetValue<string>("SigningKey");
    }
}
