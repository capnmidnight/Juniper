using System;
using System.IO;
using Juniper.HTTP.REST;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MetadataResponse>
    {
        public MetadataRequest(PanoID pano)
            : base(new Json.Factory<MetadataResponse>(), "streetview/metadata", pano)
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(PlaceName placeName)
            : base(new Json.Factory<MetadataResponse>(), "streetview/metadata", placeName)
        {
            SetContentType("application/json", "json");
        }

        public MetadataRequest(LatLngPoint location)
            : base(new Json.Factory<MetadataResponse>(), "streetview/metadata", location)
        {
            SetContentType("application/json", "json");
        }
    }
}