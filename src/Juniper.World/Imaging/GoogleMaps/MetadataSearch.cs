using Juniper.World.GIS;
using System;

namespace Juniper.World.Imaging.GoogleMaps
{
    public class MetadataSearch : Search
    {
        private static readonly Uri metadataURIBase = new Uri(baseServiceURI + "/metadata");

        public MetadataSearch(PanoID pano)
            : base(metadataURIBase, "json", pano) { }

        public MetadataSearch(string placeName)
            : base(metadataURIBase, "json", placeName) { }

        public MetadataSearch(LatLngPoint location)
            : base(metadataURIBase, "json", location) { }
    }
}