using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MetadataResponse>
    {
        public MetadataRequest(AbstractEndpoint api, PanoID pano)
            : base(api, new Json.Factory<MetadataResponse>(), "streetview/metadata", pano)
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(AbstractEndpoint api, PlaceName placeName)
            : base(api, new Json.Factory<MetadataResponse>(), "streetview/metadata", placeName)
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(AbstractEndpoint api, LatLngPoint location)
            : base(api, new Json.Factory<MetadataResponse>(), "streetview/metadata", location)
        {
            SetContentType("application/json", "json");
        }
    }
}