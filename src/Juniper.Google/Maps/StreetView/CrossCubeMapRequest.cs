using System;
using System.Threading.Tasks;

using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CrossCubeMapRequest : AbstractImageRequest
    {
        private readonly CubeMapRequest subRequest;

        public CrossCubeMapRequest(PanoID pano, Size size)
            : base(pano, size)
        {
            subRequest = new CubeMapRequest(pano, size);
        }

        public CrossCubeMapRequest(PanoID pano, int width, int height)
            : base(pano, width, height)
        {
            subRequest = new CubeMapRequest(pano, new Size(width, height));
        }

        public CrossCubeMapRequest(PlaceName placeName, Size size)
            : base(placeName, size)
        {
            subRequest = new CubeMapRequest(placeName, size);
        }

        public CrossCubeMapRequest(PlaceName placeName, int width, int height)
            : base(placeName, width, height)
        {
            subRequest = new CubeMapRequest(placeName, new Size(width, height));
        }

        public CrossCubeMapRequest(LatLngPoint location, Size size)
            : base(location, size)
        {
            subRequest = new CubeMapRequest(location, size);
        }

        public CrossCubeMapRequest(LatLngPoint location, int width, int height)
            : base(location, width, height)
        {
            subRequest = new CubeMapRequest(location, new Size(width, height));
        }

        public override bool IsCached(AbstractEndpoint api)
        {
            return subRequest.IsCached(api)
                && base.IsCached(api);
        }

        public override async Task<RawImage> Get(AbstractEndpoint api)
        {
            var cacheFile = GetCacheFile(api);
            if (base.IsCached(api))
            {
                return await Decoder.DecodeJPEGAsync(cacheFile, false);
            }
            else
            {
                var images = await subRequest.Get(api);
                var combined = await RawImage.CombineCross(images[0], images[1], images[2], images[3], images[4], images[5]);
                await Encoder.EncodeJPEGAsync(combined, cacheFile, false);
                return combined;
            }
        }

        public override Task<RawImage> Post(AbstractEndpoint api)
        {
            throw new NotImplementedException();
        }
    }
}