using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps;
using Juniper.Google.Maps.StreetView;
using Juniper.HTTP;
using Juniper.Imaging;

namespace Yarrow.Server
{
    public class YarrowImageController<T>
    {
        private readonly ImageRequest<T> imageRequest;

        public YarrowImageController(GoogleMapsRequestConfiguration gmaps, IImageDecoder<T> decoder)
        {
            imageRequest = new ImageRequest<T>(gmaps, decoder, new Size(640, 640));
        }

        [Route("/api/image\\?pano=([^/]+)")]
        public Task GetImage(HttpListenerContext context, string pano)
        {
            imageRequest.Pano = (PanoID)pano;
            return imageRequest.Proxy(context.Response);
        }
    }
}