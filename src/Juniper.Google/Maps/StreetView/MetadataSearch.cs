using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataSearch : AbstractStreetViewSearch
    {
        public MetadataSearch(PanoID pano)
            : base("streetview/metadata", "json", pano) { }

        public MetadataSearch(PlaceName placeName)
            : base("streetview/metadata", "json", placeName) { }

        public MetadataSearch(LatLngPoint location)
            : base("streetview/metadata", "json", location) { }
    }
}