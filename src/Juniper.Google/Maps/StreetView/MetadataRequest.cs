using Juniper.Json;
using Juniper.Serialization;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<IDeserializer<MetadataResponse>, MetadataResponse>
    {
        public MetadataRequest(GoogleMapsRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<MetadataResponse>(), "streetview/metadata")
        {
            SetContentType("application/json", "json");
        }
    }
}