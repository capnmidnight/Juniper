using System.Net;
using System.Threading.Tasks;

using Hjg.Pngcs;

using Juniper.Google.Maps.Tests;
using Juniper.Imaging;
using Juniper.Imaging.HjgPngcs;
using Juniper.Imaging.LibJpegNET;
using Juniper.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Google.Maps.StreetView.Tests
{
    [TestClass]
    public class StreetViewTests : ServicesTests
    {
        private LibJpegNETDecoder jpegDecoder;
        private IImageCodec<ImageLines> png;
        private HjgPngcsDecoder pngDecoder;

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            jpegDecoder = new LibJpegNETDecoder(80);

            png = new HjgPngcsCodec();
            pngDecoder = new HjgPngcsDecoder();
        }

        [TestMethod]
        public async Task JPEGImageSize()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640), jpegDecoder.ImageType)
            {
                Place = "Alexandria, VA"
            };

            var image = await cache.GetDecoded(imageRequest, jpegDecoder);
            var info = image.info;
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task PNGImageSize()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640), jpegDecoder.ImageType)
            {
                Place = "Alexandria, VA"
            };

            var rawImg = await cache.GetDecoded(imageRequest, jpegDecoder);
            var data = pngDecoder.Serialize(rawImg);
            var info = png.GetImageInfo(data);
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
            {
                Place = "Washington, DC"
            };
            var metadataDecoder = new JsonFactory().Specialize<MetadataResponse>();
            var metadata = await cache.GetDecoded(metadataRequest, metadataDecoder);
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull("2016-07", metadata.date.ToString("yyyy-MM"));
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(4096, 4096), jpegDecoder.ImageType)
            {
                Place = "Alexandria, VA"
            };

            var image = await cache.GetDecoded(imageRequest, jpegDecoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }

        [TestMethod]
        public async Task GetImageWithoutCaching()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640), jpegDecoder.ImageType)
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest.GetDecoded(jpegDecoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }
    }
}