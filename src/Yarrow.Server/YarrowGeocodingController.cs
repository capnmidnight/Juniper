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
        public Task ReverseGeocodeLatLng(HttpListenerContext context, string latLngString)
        {
            var revGeocodeRequest = new ReverseGeocodingRequest(gmaps)
            {
                Location = LatLngPoint.ParseDecimal(latLngString)
            };
            return revGeocodeRequest.Proxy(context.Response);
        }
    }
}