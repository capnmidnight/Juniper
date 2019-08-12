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
        private readonly ReverseGeocodingRequest revGeocodeRequest;

        public YarrowGeocodingController(GoogleMapsRequestConfiguration gmaps)
        {
            revGeocodeRequest = new ReverseGeocodingRequest(gmaps);
        }

        [Route("/api/geocode\\?latlng=([^/]+)")]
        public Task ReverseGeocodeLatLng(HttpListenerContext context, string latLngString)
        {
            latLngString = Uri.UnescapeDataString(latLngString);
            var latLng = LatLngPoint.ParseDecimal(latLngString);
            revGeocodeRequest.Location = latLng;
            return revGeocodeRequest.Proxy(context.Response);
        }
    }
}