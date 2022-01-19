using System.Text.RegularExpressions;

namespace Juniper.Configuration
{
    public static class IWebHostEnvironmentExt
    {
        public static Version GetVersion(this IWebHostEnvironment env, IConfiguration config)
        {
            return config.GetValue<Version>("Version");
        }
    }
}
