using System;
using System.IO;
using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MetadataResponse>
    {
        public MetadataRequest(PanoID pano)
            : base("streetview/metadata", "application/json", "json", pano) { }

        public MetadataRequest(PlaceName placeName)
            : base("streetview/metadata", "application/json", "json", placeName) { }

        public MetadataRequest(LatLngPoint location)
            : base("streetview/metadata", "application/json", "json", location) { }

        public override Func<Stream, MetadataResponse> GetDecoder(AbstractEndpoint api)
        {
            return api.DecodeObject<MetadataResponse>;
        }
    }
}