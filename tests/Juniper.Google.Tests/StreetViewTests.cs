using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps.Tests;
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
            var imageRequest = new CrossCubeMapRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Alexandria, VA");
            if (!imageRequest.IsCached)
            {
                await imageRequest.Get();
            }

            var img = decoder.Read(imageRequest.CacheFile);
            Assert.AreEqual(2560, img.dimensions.width);
            Assert.AreEqual(1920, img.dimensions.height);
        }

        [TestMethod]
        public async Task PNGImageSize()
        {
            var decoder = new JpegDecoder();
            var imageRequest = new CrossCubeMapRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Alexandria, VA");
            var rawImg = await imageRequest.Get();
            var png = new PngDecoder();
            var data = png.Serialize(rawImg);
            var img = png.Read(data, DataSource.File);
            Assert.AreEqual(2560, img.dimensions.width);
            Assert.AreEqual(1920, img.dimensions.height);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataRequest = new MetadataRequest(service, (PlaceName)"Washington, DC");
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
            var imageRequest = new ImageRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Alexandria, VA");
            var image = await imageRequest.Get();
            Assert.IsTrue(imageRequest.IsCached);
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }

        [TestMethod]
        public async Task GetImageWithoutCaching()
        {
            var decoder = new JpegDecoder();
            var imageRequest = new ImageRequest<ImageData>(noCacheService, decoder, new Size(640, 640), (PlaceName)"Alexandria, VA");
            var image = await imageRequest.Get();
            Assert.IsFalse(imageRequest.IsCached);
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }

        [TestMethod]
        public async Task GetCubeMap()
        {
            var decoder = new JpegDecoder();
            var cubeMapRequest = new CubeMapRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Washington, DC");
            var images = await cubeMapRequest.Get();
            Assert.IsTrue(cubeMapRequest.IsCached);
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.dimensions.width);
                Assert.AreEqual(640, image.dimensions.height);
            }
        }

        [TestMethod]
        public async Task SaveCubeMap6PNG()
        {
            var decoder = new JpegDecoder();
            var cubeMapRequest = new CubeMapRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Washington, DC");
            var images = await cubeMapRequest.Get();
            var concator = new JpegDecoder();
            var combined = await concator.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
            var outputFileName = Path.Combine(cacheDir.FullName, "dc6.png");
            var encoder = new PngDecoder();
            encoder.Save(outputFileName, combined);
            Assert.IsTrue(File.Exists(outputFileName));
        }

        [TestMethod]
        public async Task SaveCubeMap6JPEG()
        {
            var decoder = new JpegDecoder();
            var cubeMapRequest = new CubeMapRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Washington, DC");
            var images = await cubeMapRequest.Get();
            var concator = new JpegDecoder();
            var combined = await concator.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
            var outputFileName = Path.Combine(cacheDir.FullName, "dc6.jpeg");
            var encoder = new PngDecoder();
            encoder.Save(outputFileName, combined);
            Assert.IsTrue(File.Exists(outputFileName));
        }

        [TestMethod]
        public async Task GetCubeMapCrossJPEG()
        {
            var decoder = new JpegDecoder();
            var cubeMapRequest = new CrossCubeMapRequest<ImageData>(service, decoder, new Size(640, 640), (PlaceName)"Washington, DC");
            cubeMapRequest.CacheFile.Delete();
            var combined = await cubeMapRequest.Get();
            Assert.IsNotNull(combined);
        }
    }
}