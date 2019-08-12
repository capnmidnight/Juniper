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
        private readonly GoogleMapsRequestConfiguration gmaps;
        private readonly IImageDecoder<T> decoder;

        public YarrowImageController(GoogleMapsRequestConfiguration gmaps, IImageDecoder<T> decoder)
        {
            this.gmaps = gmaps;
            this.decoder = decoder;
        }

        [Route("/api/image\\?pano=([^/]+)&fov=(\\d+)&heading=(\\d+)&pitch=(-?\\d+)")]
        public Task GetImage(HttpListenerContext context, string pano, string fovString, string headingString, string pitchString)
        {
            if (int.TryParse(fovString, out var fov)
                && int.TryParse(headingString, out var heading)
                && int.TryParse(pitchString, out var pitch))
            {
                var imageRequest = new ImageRequest<T>(gmaps, decoder, new Size(640, 640))
                {
                    Pano = (PanoID)pano,
                    FOV = fov,
                    Heading = heading,
                    Pitch = pitch
                };
                return imageRequest.Proxy(context.Response);
            }
            else
            {
                context.Response.Error(HttpStatusCode.BadRequest, "Excepted parameters [pano:string, fov:int, heading:int, pitch:int]");
                return Task.CompletedTask;
            }
        }
    }
}