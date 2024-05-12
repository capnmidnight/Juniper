using Microsoft.Extensions.Configuration;

namespace Juniper.Configuration;

public static partial class IConfigurationExt
{
    public static string? GetGoogleAPIKey(this IConfiguration config) =>
        config.GetValue<string>("Google:APIKey");

    public static string? GetGoogleSigningKey(this IConfiguration config) =>
        config.GetValue<string>("Google:SigningKey");
}
