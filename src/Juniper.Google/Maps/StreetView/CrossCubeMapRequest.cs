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

        public CrossCubeMapRequest(PanoID pano, Size size, ImageFormat format)
            : base(pano, size)
        {
            Format = format;
            subRequest = new CubeMapRequest(pano, size);
        }

        public CrossCubeMapRequest(PanoID pano, int width, int height, ImageFormat format)
            : base(pano, width, height)
        {
            Format = format;
            subRequest = new CubeMapRequest(pano, new Size(width, height));
        }

        public CrossCubeMapRequest(PlaceName placeName, Size size, ImageFormat format)
            : base(placeName, size)
        {
            Format = format;
            subRequest = new CubeMapRequest(placeName, size);
        }

        public CrossCubeMapRequest(PlaceName placeName, int width, int height, ImageFormat format)
            : base(placeName, width, height)
        {
            Format = format;
            subRequest = new CubeMapRequest(placeName, new Size(width, height));
        }

        public CrossCubeMapRequest(LatLngPoint location, Size size, ImageFormat format)
            : base(location, size)
        {
            Format = format;
            subRequest = new CubeMapRequest(location, size);
        }

        public CrossCubeMapRequest(LatLngPoint location, int width, int height, ImageFormat format)
            : base(location, width, height)
        {
            Format = format;
            subRequest = new CubeMapRequest(location, new Size(width, height));
        }
        public ImageFormat Format { get; set; }

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
                return await Decoder.DecodeAsync(Format, cacheFile, false);
            }
            else
            {
                var images = await subRequest.Get(api);
                var combined = await RawImage.CombineCross(images[0], images[1], images[2], images[3], images[4], images[5]);
                await Encoder.EncodeAsync(Format, combined, cacheFile, false);
                return combined;
            }
        }

        public override Task<RawImage> Post(AbstractEndpoint api)
        {
            throw new NotImplementedException();
        }
    }
}