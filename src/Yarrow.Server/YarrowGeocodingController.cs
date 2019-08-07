using System;
using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.Geocoding;
using Juniper.HTTP;
using Juniper.World.GIS;

namespace Yarrow.Server
{
    public class YarrowGeocodingController
    {
        private readonly GoogleMapsRequestConfiguration gmaps;

        public YarrowGeocodingController(GoogleMapsRequestConfiguration gmaps)
        {
            this.gmaps = gmaps;
        }

        [Route("/api/geocode\\?latlng=([^/]+)")]
        public void ReverseGeocodeLatLng(HttpListenerContext context, string location)
        {
            var latlngString = Uri.UnescapeDataString(location);
            var latLng = LatLngPoint.ParseDecimal(latlngString);
            var revGeocodeRequest = new ReverseGeocodingRequest(gmaps, latLng);
            Task.WaitAll(revGeocodeRequest.Proxy(context.Response));
        }
    }
}