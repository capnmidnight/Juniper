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

        public override bool CanCache(IContentReference fileRef)
        {
            return (fileRef is IGoogleMapsRequest
                    || fileRef.ContentType == MediaType.Image.Jpeg)
                && base.CanCache(fileRef);
        }
    }
}
