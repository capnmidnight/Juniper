using System.IO;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public abstract class AbstractStreetViewRequest<ResultType> : AbstractMapsRequest<ResultType>
    {
        private PanoID pano;
        private PlaceName placeName;
        private LatLngPoint location;

        private AbstractStreetViewRequest(string path, string key)
            : base(path, Path.Combine("streetview", key), true)
        {
        }

        protected AbstractStreetViewRequest(string path, PanoID location)
            : this(path, $"pano={location}")
        {
            SetLocation(location);
        }

        protected AbstractStreetViewRequest(string path, PlaceName location)
            : this(path, $"address={location}")
        {
            SetLocation(location);
        }

        protected AbstractStreetViewRequest(string path, LatLngPoint location)
            : this(path, $"latlng={location}")
        {
            SetLocation(location);
        }

        public PanoID Pano
        {
            get { return pano; }
            set { SetLocation(value); }
        }

        public PlaceName Place
        {
            get { return placeName; }
            set { SetLocation(value); }
        }

        public LatLngPoint Location
        {
            get { return location; }
            set { SetLocation(value); }
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