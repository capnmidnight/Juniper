using System.Net;
using System.Text;
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
        public void EncodeOnePart()
        {
            var sb = new StringBuilder();
            var first = 0;
            MapTiles.LinePath.EncodePolylinePart(sb, -179.9832104, ref first);
            var encoded = sb.ToString();
            Assert.AreEqual("`~oia@", encoded);
        }

        [TestMethod]
        public void EncodePair()
        {
            var input = "38.5, -120.2";
            var expected = "_p~iF~ps|U";
            EncodePolylinePartTest(expected, input);
        }

        [TestMethod]
        public void EncodeString()
        {
            var input = "38.5, -120.2|40.7, -120.95|43.252, -126.453".Split('|');
            var expected = "_p~iF~ps|U_ulLnnqC_mqNvxq`@";
            EncodePolylinePartTest(expected, input);
        }

        private static void EncodePolylinePartTest(string expected, params string[] input)
        {
            var encoded = MapTiles.LinePath.EncodePolyline(input);
            Assert.AreEqual(expected, encoded);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataSearch = new MetadataSearch((PlaceName)"Washington, DC");
            var metadata = await service.Get(metadataSearch);
            Assert.IsTrue(service.IsCached(metadataSearch));
            Assert.AreEqual(HttpStatusCode.OK, metadata.status);
            Assert.IsNull(metadata.error_message);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull(metadata.date);
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageSearch = new ImageSearch((PlaceName)"Washington, DC", 640, 640);
            var image = await service.Get(imageSearch);
            Assert.IsTrue(service.IsCached(imageSearch));
            Assert.AreEqual(640, image.dimensions.width);
            Assert.AreEqual(640, image.dimensions.height);
        }

        [TestMethod]
        public async Task GetImage_10x()
        {
            var imageSearch = new ImageSearch((PlaceName)"Washington, DC", 640, 640);
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
            var imageSearch = new ImageSearch((PlaceName)"Washington, DC", 640, 640)
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
            var cubeMapSearch = new CubeMapSearch((PlaceName)"Washington, DC", 640, 640);
            var images = await service.Get(cubeMapSearch);
            Assert.IsTrue(service.IsCached(cubeMapSearch));
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.dimensions.width);
                Assert.AreEqual(640, image.dimensions.height);
            }
        }

        [TestMethod]
        public void GetCubeMap_10x()
        {
            var cubeMapSearch = new CubeMapSearch((PlaceName)"Washington, DC", 640, 640);
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
            var cubeMapSearch = new CubeMapSearch((PlaceName)"Washington, DC", 640, 640)
            {
                FlipImages = true
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