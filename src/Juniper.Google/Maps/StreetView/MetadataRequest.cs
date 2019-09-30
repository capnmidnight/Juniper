using Juniper.HTTP;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest
    {
        public MetadataRequest(string apiKey, string signingKey)
            : base("streetview/metadata", apiKey, signingKey, MediaType.Application.Json)
        { }
    }
}