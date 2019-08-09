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
        private readonly CrossCubeMapRequest<T> imageRequest;

        public YarrowImageController(GoogleMapsRequestConfiguration gmaps, IImageDecoder<T> decoder)
        {
            imageRequest = new CrossCubeMapRequest<T>(gmaps, decoder, new Size(640, 640));
        }

        [Route("/api/image\\?pano=([^/]+)")]
        public Task GetImage(HttpListenerContext context, string pano)
        {
            imageRequest.Pano = (PanoID)pano;
            return imageRequest.ProxyJPEG(context.Response);
        }
    }
}