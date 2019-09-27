using System.IO;

namespace Juniper.Google.Maps.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest
    {

        public MetadataRequest(string apiKey, string signingKey, DirectoryInfo cacheLocation)
            : base("streetview/metadata", apiKey, signingKey, AddPath(cacheLocation, "streetview"))
        { }

        public MetadataRequest(string apiKey, string signingKey)
            : this(apiKey, signingKey, null)
        { }
    }
}