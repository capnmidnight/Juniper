using System.IO;
using System.Threading.Tasks;

using Juniper.Image;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.StreetView
{
    public class CrossCubeMapRequest : AbstractStreetViewImageRequest
    {
        private readonly CubeMapRequest subRequest;
        private readonly IFactory<ImageData> factory;

        public CrossCubeMapRequest(GoogleMapsRequestConfiguration api, PanoID pano, Size size)
            : base(api, pano, size)
        {
            factory = (IFactory<ImageData>)deserializer;
            subRequest = new CubeMapRequest(api, pano, size);
        }

        public CrossCubeMapRequest(GoogleMapsRequestConfiguration api, PanoID pano, int width, int height)
            : base(api, pano, width, height)
        {
            factory = (IFactory<ImageData>)deserializer;
            subRequest = new CubeMapRequest(api, pano, new Size(width, height));
        }

        public CrossCubeMapRequest(GoogleMapsRequestConfiguration api, PlaceName placeName, Size size)
            : base(api, placeName, size)
        {
            factory = (IFactory<ImageData>)deserializer;
            subRequest = new CubeMapRequest(api, placeName, size);
        }

        public CrossCubeMapRequest(GoogleMapsRequestConfiguration api, PlaceName placeName, int width, int height)
            : base(api, placeName, width, height)
        {
            factory = (IFactory<ImageData>)deserializer;
            subRequest = new CubeMapRequest(api, placeName, new Size(width, height));
        }

        public CrossCubeMapRequest(GoogleMapsRequestConfiguration api, LatLngPoint location, Size size)
            : base(api, location, size)
        {
            factory = (IFactory<ImageData>)deserializer;
            subRequest = new CubeMapRequest(api, location, size);
        }

        public CrossCubeMapRequest(GoogleMapsRequestConfiguration api, LatLngPoint location, int width, int height)
            : base(api, location, width, height)
        {
            factory = (IFactory<ImageData>)deserializer;
            subRequest = new CubeMapRequest(api, location, new Size(width, height));
        }

        public override bool IsCached
        {
            get
            {
                return subRequest.IsCached
                    && base.IsCached;
            }
        }

        protected override string CacheFileName
        {
            get
            {
                var fileName = base.CacheFileName;
                var extension = Path.GetExtension(fileName);
                return Path.ChangeExtension(fileName, "cubemap" + extension);
            }
        }

        public Task<ImageData> GetJPEG(IProgress prog = null)
        {
            return Task.Run(async () =>
            {
                if (IsCached)
                {
                    return Image.JPEG.JpegFactory.Read(CacheFile.FullName);
                }
                else
                {
                    var image = await Get(prog);
                    return new ImageData(
                        image.source,
                        image.dimensions,
                        image.components,
                        ImageFormat.JPEG,
                        factory.Serialize(image, prog));
                }
            });
        }

        public override Task<ImageData> Get(IProgress prog = null)
        {
            return Task.Run(async () =>
            {
                var cacheFile = CacheFile;
                if (IsCached)
                {
                    return deserializer.Load(cacheFile, prog);
                }
                else
                {
                    var progs = prog.Split(3);
                    var images = await subRequest.Get(progs[0]);
                    var combined = await ImageData.CombineCross(images[0], images[1], images[2], images[3], images[4], images[5], progs[1]);
                    if (cacheFile != null)
                    {
                        factory.Save(cacheFile, combined);
                    }
                    progs[2]?.Report(1);
                    return combined;
                }
            });
        }
    }
}