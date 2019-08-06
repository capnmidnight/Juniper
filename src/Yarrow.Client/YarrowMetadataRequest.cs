using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Json;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Yarrow.Client
{
    public class YarrowMetadataRequest : AbstractSingleRequest<MetadataResponse>
    {
        private PanoID pano;
        private PlaceName placeName;
        private LatLngPoint location;

        public YarrowMetadataRequest(YarrowRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<MetadataResponse>(), "api/metadata", "metadata")
        {
            SetContentType("application/json", "json");
        }

        public YarrowMetadataRequest(YarrowRequestConfiguration api, PanoID pano)
            : this(api)
        {
            Pano = pano;
        }

        public YarrowMetadataRequest(YarrowRequestConfiguration api, PlaceName placeName)
            : this(api)
        {
            Place = placeName;
        }

        public YarrowMetadataRequest(YarrowRequestConfiguration api, LatLngPoint location)
            : this(api)
        {
            Location = location;
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