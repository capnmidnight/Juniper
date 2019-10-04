using System.Net;
using System.Threading.Tasks;

using Hjg.Pngcs;

using Juniper.GIS.Google.Tests;
using Juniper.Imaging;
using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.GIS.Google.StreetView.Tests
{
    [TestClass]
    public class StreetViewTests : ServicesTests
    {
        private LibJpegNETImageDataTranscoder jpegDecoder;
        private IImageCodec<ImageLines> png;
        private HjgPngcsImageDataTranscoder pngDecoder;

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            jpegDecoder = new LibJpegNETImageDataTranscoder(80);

            png = new HjgPngcsCodec();
            pngDecoder = new HjgPngcsImageDataTranscoder();
        }

        [TestMethod]
        public async Task JPEGImageSize()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await cache.Decode(imageRequest, jpegDecoder);
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

            var rawImg = await cache.Decode(imageRequest, jpegDecoder);
            var data = pngDecoder.Serialize(rawImg);
            var info = png.GetImageInfo(data);
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
            var metadata = await cache.Decode(metadataRequest, metadataDecoder);
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

            var image = await cache.Decode(imageRequest, jpegDecoder);
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