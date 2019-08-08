using Juniper.Json;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MetadataResponse>
    {
        public MetadataRequest(GoogleMapsRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<MetadataResponse>(), "streetview/metadata")
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(GoogleMapsRequestConfiguration api, PanoID pano)
            : this(api)
        {
            Pano = pano;
        }

        public MetadataRequest(GoogleMapsRequestConfiguration api, PlaceName placeName)
            : this(api)
        {
            Place = placeName;
        }

        public MetadataRequest(GoogleMapsRequestConfiguration api, LatLngPoint location)
            : this(api)
        {
            Location = location;
        }
    }
}