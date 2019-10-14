using System.IO;

using Juniper.HTTP.REST;
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

        public override bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
        {
            return (fileRef is AbstractRequest<MediaTypeT>
                    || fileRef.ContentType == MediaType.Image.Jpeg)
                && base.CanCache(fileRef);
        }
    }
}
