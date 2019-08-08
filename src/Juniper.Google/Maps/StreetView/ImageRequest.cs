using Juniper.Imaging;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class ImageRequest<T> : AbstractStreetViewImageRequest<T>
    {
        public static ImageRequest<T> Create(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, LocationTypes locationType, object value, Size size)
        {
            switch (locationType)
            {
                case LocationTypes.PanoID: return new ImageRequest<T>(api, decoder, size, (PanoID)value);
                case LocationTypes.PlaceName: return new ImageRequest<T>(api, decoder, size, (PlaceName)value);
                case LocationTypes.LatLngPoint: return new ImageRequest<T>(api, decoder, size, (LatLngPoint)value);
                default: return default;
            }
        }

        public static ImageRequest<T> Create(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, LocationTypes locationType, object value, int width, int height)
        {
            return Create(api, decoder, locationType, value, new Size(width, height));
        }

        private Heading heading;
        private Pitch pitch;

        public ImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size)
            : base(api, decoder, size) { }

        public ImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, PanoID pano)
            : base(api, decoder, size, pano) { }

        public ImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, PlaceName placeName)
            : base(api, decoder, size, placeName) { }

        public ImageRequest(GoogleMapsRequestConfiguration api, IImageDecoder<T> decoder, Size size, LatLngPoint location)
            : base(api, decoder, size, location) { }

        public Heading Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                SetQuery(nameof(heading), (int)value);
            }
        }

        public Pitch Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                SetQuery(nameof(pitch), (int)value);
            }
        }
    }
}