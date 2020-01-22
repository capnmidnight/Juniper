using System.IO;

using Juniper.IO;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsCacheLayer : FileCacheLayer
    {
        public GoogleMapsCacheLayer(DirectoryInfo cacheLocation)
            : base(cacheLocation)
        {
        }

        public GoogleMapsCacheLayer(string directoryName)
            : base(directoryName)
        {
        }

        public override bool CanCache(ContentReference fileRef)
        {
            return fileRef is IGoogleMapsRequest
                && base.CanCache(fileRef);
        }
    }
}
