using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace Juniper.Configuration;

public static partial class IConfigurationExt
{
    public const int CACHE_TIME = 24 * 60 * 60;

    public static IList<string> GetDefaultFiles(this IConfiguration config) =>
        (from file in config.GetSection("DefaultFiles").GetChildren()
        where file?.Value is not null
        select file.Value)
            .ToList();

    public static FileExtensionContentTypeProvider GetContentTypes(this IConfiguration config)
    {
        var extTypes = new FileExtensionContentTypeProvider();
        var optionalTypes = config.GetSection("ContentTypes")
            ?.GetChildren();
        if (optionalTypes is not null)
        {
            foreach (var type in optionalTypes)
            {
                if (type?.Value is not null)
                {
                    extTypes.Mappings[type.Key] = type.Value;
                }
            }
        }

        return extTypes;
    }

    public static Version? GetVersion(this IConfiguration config) =>
        config.GetValue<Version>("Version");
}
