using Microsoft.AspNetCore.StaticFiles;

namespace Juniper.Configuration
{
    public static partial class IConfigurationExt
    {
        public const int CACHE_TIME = 24 * 60 * 60;

        public static IList<string> GetDefaultFiles(this IConfiguration config)
        {
            return config.GetSection("DefaultFiles")
                ?.GetChildren()
                ?.Select(file => file.Value)
                ?.ToList();
        }

        public static FileExtensionContentTypeProvider GetContentTypes(this IConfiguration config)
        {
            var extTypes = new FileExtensionContentTypeProvider();
            var optionalTypes = config.GetSection("ContentTypes")
                ?.GetChildren();
            if (optionalTypes is object)
            {
                foreach (var type in optionalTypes)
                {
                    extTypes.Mappings[type.Key] = type.Value;
                }
            }

            return extTypes;
        }
    }
}
