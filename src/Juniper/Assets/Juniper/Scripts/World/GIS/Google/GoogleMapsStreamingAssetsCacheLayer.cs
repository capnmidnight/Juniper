using Juniper.HTTP.REST;
using Juniper.IO;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsStreamingAssetsCacheLayer : StreamingAssetsCacheLayer
    {
        public override bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
        {
            return !(fileRef is AbstractRequest<MediaTypeT>)
                && base.CanCache(fileRef);
        }
    }
}
