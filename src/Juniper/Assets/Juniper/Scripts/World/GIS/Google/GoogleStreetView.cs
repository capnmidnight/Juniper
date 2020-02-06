using Juniper.World.GIS.Google.StreetView;

namespace Juniper.World.GIS.Google
{
    public class GoogleStreetView : AbstractGoogleStreetView<MetadataResponse>
    {
        public string cachePrefix = "Juniper";

        protected override string CachePrefix
        {
            get
            {
                return cachePrefix;
            }
        }
    }
}