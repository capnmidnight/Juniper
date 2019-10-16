using Juniper.IO;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsStreamingAssetsCacheLayer : StreamingAssetsCacheLayer
    {
        public override bool CanCache(IContentReference fileRef)
        {
            return !(fileRef is IGoogleMapsRequest)
                && base.CanCache(fileRef);
        }
    }
}
