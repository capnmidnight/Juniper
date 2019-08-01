using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class ImageRequest : AbstractStreetViewImageRequest
    {
        public static ImageRequest Create(AbstractEndpoint api, LocationTypes locationType, object value, Size size)
        {
            switch (locationType)
            {
                case LocationTypes.PanoID: return new ImageRequest(api, (PanoID)value, size);
                case LocationTypes.PlaceName: return new ImageRequest(api, (PlaceName)value, size);
                case LocationTypes.LatLngPoint: return new ImageRequest(api, (LatLngPoint)value, size);
                default: return default;
            }
        }

        public static ImageRequest Create(AbstractEndpoint api, LocationTypes locationType, object value, int width, int height)
        {
            return Create(api, locationType, value, new Size(width, height));
        }

        private Heading heading;
        private Pitch pitch;

        public ImageRequest(AbstractEndpoint api, PanoID pano, Size size)
            : base(api, pano, size) { }

        public ImageRequest(AbstractEndpoint api, PanoID pano, int width, int height)
            : base(api, pano, width, height) { }

        public ImageRequest(AbstractEndpoint api, PlaceName placeName, Size size)
            : base(api, placeName, size) { }

        public ImageRequest(AbstractEndpoint api, PlaceName placeName, int width, int height)
            : base(api, placeName, width, height) { }

        public ImageRequest(AbstractEndpoint api, LatLngPoint location, Size size)
            : base(api, location, size) { }

        public ImageRequest(AbstractEndpoint api, LatLngPoint location, int width, int height)
            : base(api, location, width, height) { }

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