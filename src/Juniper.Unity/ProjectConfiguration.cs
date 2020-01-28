using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juniper.IO;
using Juniper.XR;

namespace Juniper
{
    public static class ProjectConfiguration
    {
        public static PlatformType Platform
        {
            get
            {
                var deserializer = new StringFactory();
                var source = new UnityCachingStrategy();
                var file = new ContentReference("juniper", MediaType.Text.Plain);
                if (source.IsCached(file)
                    && source.TryLoad(deserializer, file, out var platformName)
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
                var serializer = new StringFactory();
                var dest = new UnityCachingStrategy();
                var file = new ContentReference("juniper", MediaType.Text.Plain);
                dest.Save(serializer, file, value.ToString());
            }
        }
    }
}
