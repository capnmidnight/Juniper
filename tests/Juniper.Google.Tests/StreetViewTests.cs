using System.Net;
using System.Threading.Tasks;

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
        [TestMethod]
        public async Task JPEGImageSize()
        {
            var imageRequest = new ImageRequest(service, new Size(640, 640))
            {
                Place = (PlaceName)"Alexandria, VA"
            };

            var decoder = new LibJpegNETImageDataTranscoder();
            var image = await imageRequest.GetDecoded(decoder);
            var info = image.info;
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task PNGImageSize()
        {
            var imageRequest = new ImageRequest(service, new Size(640, 640))
            {
                Place = (PlaceName)"Alexandria, VA"
            };

            var jpeg = new LibJpegNETImageDataTranscoder();
            var rawImg = await imageRequest.GetDecoded(jpeg);
            var png = new HjgPngcsImageDataTranscoder();
            var data = png.Serialize(rawImg);
            var info = png.GetImageInfo(data);
            Assert.AreEqual(640, info.dimensions.width);
            Assert.AreEqual(640, info.dimensions.height);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataRequest = new MetadataRequest(service)
            {
                Place = (PlaceName)"Washington, DC"
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
                Place = (PlaceName)"Alexandria, VA"
            };
            var decoder = new LibJpegNETImageDataTranscoder();
            var image = await imageRequest.GetDecoded(decoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }

        [TestMethod]
        public async Task GetImageWithoutCaching()
        {
            var imageRequest = new ImageRequest(noCacheService, new Size(640, 640))
            {
                Place = (PlaceName)"Alexandria, VA"
            };
            var decoder = new LibJpegNETImageDataTranscoder();
            var image = await imageRequest.GetDecoded(decoder);
            Assert.AreEqual(640, image.info.dimensions.width);
            Assert.AreEqual(640, image.info.dimensions.height);
        }
    }
}