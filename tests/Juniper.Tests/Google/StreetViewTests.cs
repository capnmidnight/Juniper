using System.Net;
using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.World.GIS.Google.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.World.GIS.Google.StreetView.Tests
{
    [TestClass]
    public class StreetViewTests : ServicesTests
    {
        private IImageCodec<ImageData> jpegDecoder;
        private IImageCodec<ImageData> pngDecoder;

        [TestInitialize]
        public override void Init()
        {
            base.Init();

            jpegDecoder = new TranscoderCodec<BitMiracle.LibJpeg.JpegImage, ImageData>(
                new LibJpegNETCodec(80),
                new LibJpegNETImageDataTranscoder());

            pngDecoder = new TranscoderCodec<Hjg.Pngcs.ImageLines, ImageData>(
                new HjgPngcsCodec(),
                new HjgPngcsImageDataTranscoder());
        }

        [TestMethod]
        public async Task JPEGImageSize()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await cache.Load(imageRequest, jpegDecoder);
            var info = image.info;
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task PNGImageSize()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var rawImg = await cache.Load(imageRequest, jpegDecoder);
            var data = pngDecoder.Serialize(rawImg);
            var info = Imaging.ImageInfo.ReadPNG(data);
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
            {
                Place = "Washington, DC"
            };
            var metadata = await cache.Load(metadataRequest, metadataDecoder);
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull("2016-07", metadata.date.ToString("yyyy-MM"));
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(4096, 4096))
            {
                Place = "Alexandria, VA"
            };

            var image = await cache.Load(imageRequest, jpegDecoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }

        [TestMethod]
        public async Task GetImageWithoutCaching()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest.Decode(jpegDecoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }
    }
}