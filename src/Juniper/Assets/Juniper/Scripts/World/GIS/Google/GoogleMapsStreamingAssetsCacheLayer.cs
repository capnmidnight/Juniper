using Juniper.IO;

namespace Juniper.World.GIS.Google
{
    public class GoogleMapsStreamingAssetsCacheLayer : StreamingAssetsCacheLayer
    {
        public GoogleMapsStreamingAssetsCacheLayer()
            : base()
        { }

        public GoogleMapsStreamingAssetsCacheLayer(string prefix)
            : base(prefix)
        { }

        public override bool CanCache(ContentReference fileRef)
        {
            return !(fileRef is IGoogleMapsRequest)
                && base.CanCache(fileRef);
        }
    }
}
