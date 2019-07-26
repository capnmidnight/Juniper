using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps.Tests;
using Juniper.Image;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Google.Maps.StreetView.Tests
{
    [TestClass]
    public class StreetViewTests : ServicesTests
    {
        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataRequest = new MetadataRequest((PlaceName)"Washington, DC");
            var metadata = await service.Get(metadataRequest);
            Assert.IsTrue(service.IsCached(metadataRequest));
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull(metadata.date);
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageRequest = new ImageRequest((PlaceName)"Alexandria, VA", 640, 640);
            var image = await service.Get(imageRequest);
            Assert.IsTrue(service.IsCached(imageRequest));
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }

        [TestMethod]
        public async Task GetCubeMap()
        {
            var cubeMapRequest = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapRequest);
            Assert.IsTrue(service.IsCached(cubeMapRequest));
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.dimensions.width);
                Assert.AreEqual(640, image.dimensions.height);
            }
        }

        [TestMethod]
        public async Task SaveCubeMap6PNG()
        {
            var cubeMapRequest = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapRequest);
            var combined = await RawImage.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
            var outputFileName = Path.Combine(cacheDir.FullName, "dc6.png");
            var encoder = new Image.PNG.Factory();
            await encoder.EncodeAsync(combined, outputFileName, false);
            Assert.IsTrue(File.Exists(outputFileName));
        }

        [TestMethod]
        public async Task SaveCubeMap6JPEG()
        {
            var cubeMapRequest = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapRequest);
            var combined = await RawImage.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
            var outputFileName = Path.Combine(cacheDir.FullName, "dc6.jpeg");
            var encoder = new Image.PNG.Factory();
            await encoder.EncodeAsync(combined, outputFileName, false);
            Assert.IsTrue(File.Exists(outputFileName));
        }

        [TestMethod]
        public async Task GetCubeMapCrossPNG()
        {
            var cubeMapRequest = new CrossCubeMapRequest((PlaceName)"Washington, DC", 640, 640, ImageFormat.PNG);
            var combined = await service.Get(cubeMapRequest);
            Assert.IsNotNull(combined);
        }

        [TestMethod]
        public async Task GetCubeMapCrossJPEG()
        {
            var cubeMapRequest = new CrossCubeMapRequest((PlaceName)"Washington, DC", 640, 640, ImageFormat.JPEG);
            cubeMapRequest.GetCacheFile(service).Delete();
            var combined = await service.Get(cubeMapRequest);
            Assert.IsNotNull(combined);
        }
    }
}