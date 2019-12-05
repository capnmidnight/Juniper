using System.IO;

using Juniper.IO;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsCachingStrategy : CachingStrategy
    {
        public GoogleMapsCachingStrategy(string baseCachePath)
        {
            var gmapsCacheDirName = Path.Combine(baseCachePath, "GoogleMaps");
            var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
            AppendLayer(new GoogleMapsCacheLayer(gmapsCacheDir));
            AppendLayer(new GoogleMapsStreamingAssetsCacheLayer());
        }
    }
}
