using System;
using System.IO;
using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MetadataResponse>
    {
        public MetadataRequest(PanoID pano)
            : base("streetview/metadata", pano)
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(PlaceName placeName)
            : base("streetview/metadata", placeName)
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(LatLngPoint location)
            : base("streetview/metadata", location)
        {
            SetContentType("application/json", "json");
        }

        public override Func<Stream, MetadataResponse> GetDecoder(AbstractEndpoint api)
        {
            return api.DecodeObject<MetadataResponse>;
        }
    }
}