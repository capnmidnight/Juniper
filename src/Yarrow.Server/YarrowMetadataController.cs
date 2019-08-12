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
            if (LatLngPoint.TryParseDecimal(latLngString, out var point))
            {
                var metadataRequest = new MetadataRequest(gmaps)
                {
                    Location = point
                };
                return metadataRequest.Proxy(context.Response);
            }
            else
            {
                context.Response.Error(HttpStatusCode.BadRequest, "Excepted parameter [latlng:(decimal latitude, decimal longitude)]");
                return Task.CompletedTask;
            }
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