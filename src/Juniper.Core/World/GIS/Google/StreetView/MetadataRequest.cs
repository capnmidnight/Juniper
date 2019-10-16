namespace Juniper.World.GIS.Google.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MediaType.Application>
    {
        public MetadataRequest(string apiKey, string signingKey)
            : base("streetview/metadata", apiKey, signingKey, Juniper.MediaType.Application.Json)
        { }
    }
}