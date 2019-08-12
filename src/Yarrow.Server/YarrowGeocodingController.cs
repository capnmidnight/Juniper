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
            if (LatLngPoint.TryParseDecimal(latLngString, out var point))
            {
                var revGeocodeRequest = new ReverseGeocodingRequest(gmaps)
                {
                    Location = point
                };
                return revGeocodeRequest.Proxy(context.Response);
            }
            else
            {
                context.Response.Error(HttpStatusCode.BadRequest, "Excepted parameter [latlng:(decimal latitude, decimal longitude)]");
                return Task.CompletedTask;
            }
        }
    }
}