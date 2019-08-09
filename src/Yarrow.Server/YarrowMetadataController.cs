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
        private readonly MetadataRequest metadataRequest;

        public YarrowMetadataController(GoogleMapsRequestConfiguration gmaps)
        {
            metadataRequest = new MetadataRequest(gmaps);
        }

        [Route("/api/metadata\\?location=([^/]+)")]
        public Task GetMetadataFromPlaceName(HttpListenerContext context, string locationString)
        {
            metadataRequest.Place = (PlaceName)locationString;
            return metadataRequest.Proxy(context.Response);
        }

        [Route("/api/metadata\\?latlng=([^/]+)")]
        public Task GetMetadataFromLatLng(HttpListenerContext context, string latLngString)
        {
            latLngString = Uri.UnescapeDataString(latLngString);
            var latLng = LatLngPoint.ParseDecimal(latLngString);
            metadataRequest.Location = latLng;
            return metadataRequest.Proxy(context.Response);
        }

        [Route("/api/metadata\\?pano=([^/]+)")]
        public Task GetMetadataFromPano(HttpListenerContext context, string panoString)
        {
            metadataRequest.Pano = (PanoID)panoString;
            return metadataRequest.Proxy(context.Response);
        }
    }
}