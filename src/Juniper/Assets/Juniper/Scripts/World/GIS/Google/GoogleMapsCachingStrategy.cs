using System.IO;

using Juniper.IO;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsCachingStrategy : CachingStrategy
    {
        public GoogleMapsCachingStrategy(string baseCachePath)
        {
            var gmapsCacheDir = new DirectoryInfo(baseCachePath);
            AppendLayer(new GoogleMapsCacheLayer(gmapsCacheDir));
            AppendLayer(new GoogleMapsStreamingAssetsCacheLayer());
        }

        public GoogleMapsCachingStrategy(string filePrefix, string baseCachePath)
        {
            var gmapsCacheDirName = Path.Combine(baseCachePath, filePrefix);
            var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
            AppendLayer(new GoogleMapsCacheLayer(gmapsCacheDir));
            AppendLayer(new GoogleMapsStreamingAssetsCacheLayer(filePrefix));
        }
    }
}
