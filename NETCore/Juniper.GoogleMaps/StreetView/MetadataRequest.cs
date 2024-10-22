using System.Net.Http;

namespace Juniper.World.GIS.Google.StreetView
{
    public class MetadataRequest : AbstractStreetViewRequest<MediaType.Application>
    {
        public MetadataRequest(HttpClient http, string apiKey, string signingKey)
            : base(http, "streetview/metadata", apiKey, signingKey, Juniper.MediaType.Application_Json)
        { }

        protected override string InternalCacheID
        {
            get
            {
                if (!string.IsNullOrEmpty(Pano))
                {
                    return Pano;
                }

                return base.InternalCacheID;
            }
        }
    }
}