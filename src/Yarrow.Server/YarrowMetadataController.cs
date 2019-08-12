using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP;
using Juniper.World.GIS;

namespace Yarrow.Server
{
    public class YarrowMetadataController
    {
        private readonly GoogleMapsRequestConfiguration gmaps;

        public YarrowMetadataController(GoogleMapsRequestConfiguration gmaps)
        {
            this.gmaps = gmaps;
        }

        [Route("/api/metadata\\?location=([^/]+)")]
        public Task GetMetadataFromPlaceName(HttpListenerContext context, string locationString)
        {
            var metadataRequest = new MetadataRequest(gmaps)
            {
                Place = (PlaceName)locationString
            };
            return metadataRequest.Proxy(context.Response);
        }

        [Route("/api/metadata\\?latlng=([^/]+)")]
        public Task GetMetadataFromLatLng(HttpListenerContext context, string latLngString)
        {
            var metadataRequest = new MetadataRequest(gmaps)
            {
                Location = LatLngPoint.ParseDecimal(latLngString)
            };
            return metadataRequest.Proxy(context.Response);
        }

        [Route("/api/metadata\\?pano=([^/]+)")]
        public Task GetMetadataFromPano(HttpListenerContext context, string panoString)
        {
            var metadataRequest = new MetadataRequest(gmaps)
            {
                Pano = (PanoID)panoString
            };
            return metadataRequest.Proxy(context.Response);
        }
    }
}