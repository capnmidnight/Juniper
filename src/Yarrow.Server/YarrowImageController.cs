using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP;
using Juniper.Image;

namespace Yarrow.Server
{
    public class YarrowImageController
    {
        private readonly GoogleMapsRequestConfiguration gmaps;
        private readonly Size imageSize;

        public YarrowImageController(GoogleMapsRequestConfiguration gmaps)
        {
            this.gmaps = gmaps;
            imageSize = new Size(640, 640);
        }

        [Route("/api/image\\?pano=([^/]+)")]
        public void GetImage(HttpListenerContext context, string pano)
        {
            var imageRequest = new CrossCubeMapRequest(gmaps, (PanoID)pano, imageSize);
            Task.WaitAll(imageRequest.ProxyJPEG(context.Response));
        }
    }
}