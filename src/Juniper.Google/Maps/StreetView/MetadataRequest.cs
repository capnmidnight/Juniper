namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest
    {
        public MetadataRequest(GoogleMapsRequestConfiguration api)
            : base(api, "streetview/metadata")
        {
        }
    }
}