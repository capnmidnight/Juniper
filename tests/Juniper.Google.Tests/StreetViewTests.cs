using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps.Tests;
using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.Imaging.JPEG;
using Juniper.Imaging.PNG;
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
            var decoder = new JpegDecoder();
            var imageRequest = new ImageRequest<ImageData>(service, decoder, new Size(640, 640))
            {
                Place = (PlaceName)"Alexandria, VA"
            };

            if (!imageRequest.IsCached)
            {
                await imageRequest.Get();
            }

            var img = decoder.Read(imageRequest.CacheFile);
            Assert.AreEqual(640, img.dimensions.width);
            Assert.AreEqual(640, img.dimensions.height);
        }

        [TestMethod]
        public async Task PNGImageSize()
        {
            var decoder = new JpegDecoder();
            var imageRequest = new ImageRequest<ImageData>(service, decoder, new Size(640, 640))
            {
                Place = (PlaceName)"Alexandria, VA"
            };
            var rawImg = await imageRequest.Get();
            var png = new PngDecoder();
            var data = png.Serialize(rawImg);
            var img = png.Read(data, DataSource.File);
            Assert.AreEqual(640, img.dimensions.width);
            Assert.AreEqual(640, img.dimensions.height);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataRequest = new MetadataRequest(service)
            {
                Place = (PlaceName)"Washington, DC"
            };
            var metadata = await metadataRequest.Get();
            Assert.IsTrue(metadataRequest.IsCached);
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull("2016-07", metadata.date.ToString("yyyy-MM"));
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var decoder = new JpegDecoder();
            var imageRequest = new ImageRequest<ImageData>(service, decoder, new Size(4096, 4096))
            {
                Place = (PlaceName)"Alexandria, VA"
            };
            var image = await imageRequest.Get();
            Assert.IsTrue(imageRequest.IsCached);
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }

        [TestMethod]
        public async Task GetImageWithoutCaching()
        {
            var decoder = new JpegDecoder();
            var imageRequest = new ImageRequest<ImageData>(noCacheService, decoder, new Size(640, 640))
            {
                Place = (PlaceName)"Alexandria, VA"
            };
            var image = await imageRequest.Get();
            Assert.IsFalse(imageRequest.IsCached);
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }
    }
}