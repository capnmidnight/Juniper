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
        private PlaceName location;
        private LatLngPoint latlng;

        public YarrowMetadataRequest(YarrowRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<MetadataResponse>(), "api/metadata", "metadata")
        {
            SetContentType("application/json", "json");
        }

        public PlaceName Location
        {
            get { return location; }
            set
            {
                location = value;
                latlng = default;
                SetQuery(nameof(location), (string)location);
            }
        }

        public LatLngPoint LatLng
        {
            get { return latlng; }
            set
            {
                location = default;
                latlng = value;
                SetQuery(nameof(latlng), (string)latlng);
            }
        }
    }
}