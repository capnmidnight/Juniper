using System;

using Juniper.IO;
using Juniper.XR;

namespace Juniper
{
    public static class ProjectConfiguration
    {
        private static readonly ICacheDestinationLayer CACHE = new StreamingAssetsCacheLayer();
        private static readonly IFactory<string, MediaType.Text> FACTORY = new StringFactory();
        private static readonly ContentReference FILE = new ContentReference("juniper", MediaType.Text.Plain);

        public static PlatformType Platform
        {
            get
            {
                if (CACHE.IsCached(FILE)
                    && CACHE.TryLoad(FACTORY, FILE, out var platformName)
                    && Enum.TryParse<PlatformType>(platformName, out var platform))
                {
                    return platform;
                }
                else
                {
                    return PlatformType.None;
                }
            }

            set
            {
                CACHE.Save(FACTORY, FILE, value.ToString());
            }
        }
    }
}
