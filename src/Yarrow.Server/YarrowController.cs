using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP;

namespace Yarrow.Server
{
    public class YarrowController
    {
        private readonly GoogleMapsController gmaps;

        public YarrowController(GoogleMapsController gmaps)
        {
            this.gmaps = gmaps;
        }

        [Route("/api/metadata\\?location=([^/]+)")]
        public void GetMetadata(HttpListenerContext context, string location)
        {
            Task.WaitAll(gmaps.ProxyMetadata(context.Response, (PlaceName)location));
        }

        [Route("/api/image\\?pano=([^/]+)")]
        public void GetImage(HttpListenerContext context, string pano)
        {
            Task.WaitAll(gmaps.ProxyImage(context.Response, (PanoID)pano));
        }

    }
}