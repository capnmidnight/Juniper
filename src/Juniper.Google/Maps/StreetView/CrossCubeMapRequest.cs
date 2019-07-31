using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CrossCubeMapRequest : AbstractImageRequest
    {
        private readonly CubeMapRequest subRequest;
        private readonly IFactory<RawImage> factory;

        public CrossCubeMapRequest(PanoID pano, Size size)
            : base(pano, size)
        {
            factory = (IFactory<RawImage>)deserializer;
            subRequest = new CubeMapRequest(pano, size);
        }

        public CrossCubeMapRequest(PanoID pano, int width, int height)
            : base(pano, width, height)
        {
            factory = (IFactory<RawImage>)deserializer;
            subRequest = new CubeMapRequest(pano, new Size(width, height));
        }

        public CrossCubeMapRequest(PlaceName placeName, Size size)
            : base(placeName, size)
        {
            factory = (IFactory<RawImage>)deserializer;
            subRequest = new CubeMapRequest(placeName, size);
        }

        public CrossCubeMapRequest(PlaceName placeName, int width, int height)
            : base(placeName, width, height)
        {
            factory = (IFactory<RawImage>)deserializer;
            subRequest = new CubeMapRequest(placeName, new Size(width, height));
        }

        public CrossCubeMapRequest(LatLngPoint location, Size size)
            : base(location, size)
        {
            factory = (IFactory<RawImage>)deserializer;
            subRequest = new CubeMapRequest(location, size);
        }

        public CrossCubeMapRequest(LatLngPoint location, int width, int height)
            : base(location, width, height)
        {
            factory = (IFactory<RawImage>)deserializer;
            subRequest = new CubeMapRequest(location, new Size(width, height));
        }

        public override bool IsCached(AbstractEndpoint api)
        {
            return subRequest.IsCached(api)
                && base.IsCached(api);
        }

        protected override string GetChacheFileName(AbstractEndpoint api)
        {
            var fileName = base.GetChacheFileName(api);
            var extension = Path.GetExtension(fileName);
            return Path.ChangeExtension(fileName, "cubemap" + extension);
        }

        public override async Task<RawImage> Get(AbstractEndpoint api)
        {
            var cacheFile = GetCacheFile(api);
            if (IsCached(api))
            {
                return deserializer.Deserialize(cacheFile);
            }
            else
            {
                var images = await subRequest.Get(api);
                var combined = await RawImage.CombineCross(images[0], images[1], images[2], images[3], images[4], images[5]);
                factory.Serialize(cacheFile, combined);
                return combined;
            }
        }

        public async Task<byte[]> GetJPEG(AbstractEndpoint api)
        {
            var cacheFile = GetCacheFile(api);

            if (!IsCached(api))
            {
                var images = await subRequest.Get(api);
                var combined = await RawImage.CombineCross(images[0], images[1], images[2], images[3], images[4], images[5]);
                factory.Serialize(cacheFile, combined);
            }

            return File.ReadAllBytes(cacheFile.FullName);
        }
    }
}