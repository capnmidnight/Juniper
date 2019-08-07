using System;
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
        public void GetMetadataFromPlaceName(HttpListenerContext context, string location)
        {
            var metadataRequest = new MetadataRequest(gmaps, (PlaceName)location);
            Task.WaitAll(metadataRequest.Proxy(context.Response));
        }

        [Route("/api/metadata\\?latlng=([^/]+)")]
        public void GetMetadataFromLatLng(HttpListenerContext context, string location)
        {
            var latlngString = Uri.UnescapeDataString(location);
            var latLng = LatLngPoint.ParseDecimal(latlngString);
            var metadataRequest = new MetadataRequest(gmaps, latLng);
            Task.WaitAll(metadataRequest.Proxy(context.Response));
        }

        [Route("/api/metadata\\?pano=([^/]+)")]
        public void GetMetadataFromPano(HttpListenerContext context, string panoString)
        {
            var pano = (PanoID)panoString;
            var metadataRequest = new MetadataRequest(gmaps, pano);
            Task.WaitAll(metadataRequest.Proxy(context.Response));
        }
    }
}