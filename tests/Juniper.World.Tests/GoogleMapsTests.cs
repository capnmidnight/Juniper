using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Image;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Google.Maps.StreetView.Tests
{
    [TestClass]
    public class GoogleMapsTests
    {
        string cacheDirName;
        API gmaps;

        [TestInitialize]
        public void Init()
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFile = Path.Combine(cacheDirName, "keys.txt");
            var lines = File.ReadAllLines(keyFile);
            var apiKey = lines[0];
            var signingKey = lines[1];
            var json = new Json.JsonFactory();
            gmaps = new API(json, apiKey, signingKey, cacheDir);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataSearch = new MetadataSearch((PlaceName)"Washington, DC");
            var metadata = await gmaps.Get(metadataSearch);
            Assert.IsTrue(gmaps.IsCached(metadataSearch));
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
            var image = await gmaps.Get(imageSearch);
            Assert.IsTrue(gmaps.IsCached(imageSearch));
            Assert.AreEqual(640, image.width);
            Assert.AreEqual(640, image.height);
        }

        [TestMethod]
        public async Task GetImage_10x()
        {
            var imageSearch = new ImageSearch((PlaceName)"Washington, DC", 640, 640);
            var tasks = new Task<RawImage>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = gmaps.Get(imageSearch);
            }
            var images = await Task.WhenAll(tasks);
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.width);
                Assert.AreEqual(640, image.height);
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
                tasks[i] = gmaps.Get(imageSearch);
            }
            var images = await Task.WhenAll(tasks);
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.width);
                Assert.AreEqual(640, image.height);
            }
        }

        [TestMethod]
        public async Task GetCubeMap()
        {
            var cubeMapSearch = new CubeMapSearch((PlaceName)"Washington, DC", 640, 640);
            var images = await gmaps.Get(cubeMapSearch);
            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.width);
                Assert.AreEqual(640, image.height);
            }
        }

        [TestMethod]
        public void GetCubeMap_10x()
        {
            var cubeMapSearch = new CubeMapSearch((PlaceName)"Washington, DC", 640, 640);

            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            var tasks = new Task<RawImage[]>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = gmaps.Get(cubeMapSearch);
            }

            Task.WaitAll(tasks);

            foreach (var task in tasks)
            {
                var images = task.Result;
                Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
                foreach (var image in images)
                {
                    Assert.AreEqual(640, image.width);
                    Assert.AreEqual(640, image.height);
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

            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            var tasks = new Task<RawImage[]>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = gmaps.Get(cubeMapSearch);
            }

            Task.WaitAll(tasks);

            foreach(var task in tasks)
            {
                var images = task.Result;
                Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
                foreach (var image in images)
                {
                    Assert.AreEqual(640, image.width);
                    Assert.AreEqual(640, image.height);
                }
            }
        }
    }
}
