using System.Net;
using System.Threading.Tasks;

using Hjg.Pngcs;

using Juniper.Google.Maps.Tests;
using Juniper.Imaging;
using Juniper.Imaging.HjgPngcs;
using Juniper.Imaging.LibJpegNET;
using Juniper.Json;
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
            var imageRequest = new ImageRequest(service, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest.GetDecoded(jpegDecoder);
            var info = image.info;
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task PNGImageSize()
        {
            var imageRequest = new ImageRequest(service, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var rawImg = await imageRequest.GetDecoded(jpegDecoder);
            var data = pngDecoder.Serialize(rawImg);
            var info = png.GetImageInfo(data);
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataRequest = new MetadataRequest(service)
            {
                Place = "Washington, DC"
            };
            var metadata = await metadataRequest.GetDecoded(new JsonFactory().Specialize<MetadataResponse>());
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull("2016-07", metadata.date.ToString("yyyy-MM"));
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageRequest = new ImageRequest(service, new Size(4096, 4096))
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest.GetDecoded(jpegDecoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }

        [TestMethod]
        public async Task GetImageWithoutCaching()
        {
            var imageRequest = new ImageRequest(noCacheService, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest.GetDecoded(jpegDecoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }
    }
}