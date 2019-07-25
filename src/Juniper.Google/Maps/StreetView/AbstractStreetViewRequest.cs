using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewRequest<ResultType> : AbstractMapsRequest<ResultType>
    {
        private PanoID pano;
        private PlaceName placeName;
        private LatLngPoint location;

        private AbstractStreetViewRequest(string path, string acceptType, string extension, string key)
            : base(path, key, acceptType, extension, true)
        {
        }

        protected AbstractStreetViewRequest(string path, string acceptType, string extension, PanoID location)
            : this(path, acceptType, extension, $"pano={location}")
        {
            SetLocation(location);
        }

        protected AbstractStreetViewRequest(string path, string acceptType, string extension, PlaceName location)
            : this(path, acceptType, extension, $"address={location}")
        {
            SetLocation(location);
        }

        protected AbstractStreetViewRequest(string path, string acceptType, string extension, LatLngPoint location)
            : this(path, acceptType, extension, $"latlng={location}")
        {
            SetLocation(location);
        }

        public PanoID Pano
        {
            get
            {
                return pano;
            }
            set
            {
                SetLocation(value);
            }
        }

        public PlaceName Place
        {
            get
            {
                return placeName;
            }
            set
            {
                SetLocation(value);
            }
        }

        public LatLngPoint Location
        {
            get
            {
                return location;
            }
            set
            {
                SetLocation(value);
            }
        }

        public void SetLocation(PanoID location)
        {
            placeName = default;
            this.location = default;
            pano = location;
            SetQuery(nameof(pano), (string)location);
        }

        public void SetLocation(PlaceName location)
        {
            placeName = location;
            this.location = default;
            pano = default;
            SetQuery(nameof(location), (string)location);
        }

        public void SetLocation(LatLngPoint location)
        {
            placeName = default;
            this.location = location;
            pano = default;
            SetQuery(nameof(location), (string)location);
        }
    }
}