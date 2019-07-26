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
            var metadataSearch = new MetadataRequest((PlaceName)"Washington, DC");
            var metadata = await service.Get(metadataSearch);
            Assert.IsTrue(service.IsCached(metadataSearch));
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull(metadata.date);
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageSearch = new ImageRequest((PlaceName)"Washington, DC", 640, 640);
            var image = await service.Get(imageSearch);
            Assert.IsTrue(service.IsCached(imageSearch));
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }

        [TestMethod]
        public async Task GetImage_10x()
        {
            var imageSearch = new ImageRequest((PlaceName)"Washington, DC", 640, 640);
            var tasks = new Task<RawImage>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = service.Get(imageSearch);
            }
            var images = await Task.WhenAll(tasks);
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.dimensions.width);
                Assert.AreEqual(640, image.dimensions.height);
            }
        }

        [TestMethod]
        public async Task GetImageWithFlip_10x()
        {
            var imageSearch = new ImageRequest((PlaceName)"Washington, DC", 640, 640)
            {
                FlipImage = true
            };
            var tasks = new Task<RawImage>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = service.Get(imageSearch);
            }
            var images = await Task.WhenAll(tasks);
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.dimensions.width);
                Assert.AreEqual(640, image.dimensions.height);
            }
        }

        [TestMethod]
        public async Task GetCubeMap()
        {
            var cubeMapSearch = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapSearch);
            Assert.IsTrue(service.IsCached(cubeMapSearch));
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.dimensions.width);
                Assert.AreEqual(640, image.dimensions.height);
            }
        }

        [TestMethod]
        public async Task SaveCubeMap6PNG()
        {
            var cubeMapSearch = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapSearch);
            var combined = await RawImage.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
            var outputFileName = Path.Combine(cacheDir.FullName, "dc6.png");
            await Encoder.EncodePNGAsync(combined, outputFileName, false);
            Assert.IsTrue(File.Exists(outputFileName));
        }

        [TestMethod]
        public async Task SaveCubeMap6JPEG()
        {
            var cubeMapSearch = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapSearch);
            var combined = await RawImage.Combine6Squares(images[0], images[1], images[2], images[3], images[4], images[5]);
            var outputFileName = Path.Combine(cacheDir.FullName, "dc6.jpeg");
            await Encoder.EncodePNGAsync(combined, outputFileName, false);
            Assert.IsTrue(File.Exists(outputFileName));
        }

        [TestMethod]
        public async Task SaveCubeMapCrossPNG()
        {
            var cubeMapSearch = new CrossCubeMapRequest((PlaceName)"Washington, DC", 640, 640, ImageFormat.PNG);
            var combined = await service.Get(cubeMapSearch);
            Assert.IsNotNull(combined);
        }

        [TestMethod]
        public async Task SaveCubeMapCrossJPEG()
        {
            var cubeMapSearch = new CrossCubeMapRequest((PlaceName)"Washington, DC", 640, 640, ImageFormat.JPEG);
            var combined = await service.Get(cubeMapSearch);
            Assert.IsNotNull(combined);
        }

        [TestMethod]
        public void GetCubeMap_10x()
        {
            var cubeMapSearch = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640);
            var tasks = new Task<RawImage[]>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = service.Get(cubeMapSearch);
            }

            Task.WaitAll(tasks);

            foreach (var task in tasks)
            {
                var images = task.Result;
                Assert.IsTrue(service.IsCached(cubeMapSearch));
                foreach (var image in images)
                {
                    Assert.AreEqual(640, image.dimensions.width);
                    Assert.AreEqual(640, image.dimensions.height);
                }
            }
        }

        [TestMethod]
        public void GetCubeMapWithFlip_10x()
        {
            var cubeMapSearch = new CubeMapRequest((PlaceName)"Washington, DC", 640, 640)
            {
                FlipImage = true
            };
            var tasks = new Task<RawImage[]>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = service.Get(cubeMapSearch);
            }

            Task.WaitAll(tasks);

            foreach (var task in tasks)
            {
                var images = task.Result;
                Assert.IsTrue(service.IsCached(cubeMapSearch));
                foreach (var image in images)
                {
                    Assert.AreEqual(640, image.dimensions.width);
                    Assert.AreEqual(640, image.dimensions.height);
                }
            }
        }
    }
}