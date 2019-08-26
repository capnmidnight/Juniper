using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewRequest : AbstractGoogleMapsRequest
    {
        private PanoID pano;
        private PlaceName placeName;
        private LatLngPoint location;
        private int radius;

        protected AbstractStreetViewRequest(GoogleMapsRequestConfiguration api, string path)
            : base(api, path, "streetview", true)
        {
        }

        public PanoID Pano
        {
            get { return pano; }
            set
            {
                placeName = default;
                location = default;
                pano = value;
                RemoveQuery(nameof(location));
                SetQuery(nameof(pano), (string)value);
            }
        }

        public PlaceName Place
        {
            get { return placeName; }
            set
            {
                placeName = value;
                location = default;
                pano = default;
                RemoveQuery(nameof(pano));
                SetQuery(nameof(location), (string)value);
            }
        }

        public LatLngPoint Location
        {
            get { return location; }
            set
            {
                placeName = default;
                location = value;
                pano = default;
                RemoveQuery(nameof(pano));
                SetQuery(nameof(location), (string)value);
            }
        }

        public int Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                SetQuery(nameof(radius), radius);
            }
        }
    }
}