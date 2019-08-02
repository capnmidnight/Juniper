using Juniper.HTTP.REST;
using Juniper.Json;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MetadataResponse>
    {
        private MetadataRequest(AbstractEndpoint api)
            : base(api, new JsonFactory().Specialize<MetadataResponse>(), "streetview/metadata")
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(AbstractEndpoint api, PanoID pano)
            : this(api)
        {
            Pano = pano;
        }

        public MetadataRequest(AbstractEndpoint api, PlaceName placeName)
            : this(api)
        {
            Place = placeName;
        }

        public MetadataRequest(AbstractEndpoint api, LatLngPoint location)
            : this(api)
        {
            Location = location;
        }
    }
}