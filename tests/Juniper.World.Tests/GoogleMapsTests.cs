using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Juniper.Image;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.World.Imaging.Tests
{
    [TestClass]
    public class GoogleMapsTests
    {
        string cacheDirName;
        GoogleMaps gmaps;

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
            gmaps = new GoogleMaps(json, apiKey, signingKey, cacheDir);
        }

        [TestMethod]
        public async Task GetMetadata()
        {
            var metadataSearch = new GoogleMaps.MetadataSearch("Washington, DC");
            Assert.IsTrue(gmaps.IsCached(metadataSearch));
            var metadata = await gmaps.Get(metadataSearch);
            Assert.IsTrue(gmaps.IsCached(metadataSearch));
            Assert.AreEqual(GoogleMaps.StatusCode.OK, metadata.status);
            Assert.IsNull(metadata.error_message);
            Assert.IsNotNull(metadata.copyright);
            Assert.IsNotNull(metadata.date);
            Assert.IsNotNull(metadata.location);
            Assert.IsNotNull(metadata.pano_id);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var imageSearch = new GoogleMaps.ImageSearch("Washington, DC", 640, 640);
            Assert.IsTrue(gmaps.IsCached(imageSearch));
            var image = await gmaps.Get(imageSearch);
            Assert.IsTrue(gmaps.IsCached(imageSearch));
            Assert.AreEqual(640, image.width);
            Assert.AreEqual(640, image.height);
        }

        [TestMethod]
        public async Task GetImage_10x()
        {
            var imageSearch = new GoogleMaps.ImageSearch("Washington, DC", 640, 640);
            Assert.IsTrue(gmaps.IsCached(imageSearch));
            var tasks = new Task<RawImage>[10];
            for(int i = 0; i < tasks.Length; ++i)
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
        public async Task Flip_10x()
        {
            var imageSearch = new GoogleMaps.ImageSearch("Washington, DC", 640, 640);
            Assert.IsTrue(gmaps.IsCached(imageSearch));
            var image = await gmaps.Get(imageSearch);
            var images = new RawImage[10];
            for(int i = 0; i < images.Length; ++i)
            {
                images[i] = image.CreateCopy();
            }
            var tasks = images.Select(img => img.FlipAsync()).ToArray();
            Task.WaitAll(tasks);
            foreach (var img in images)
            {
                Assert.AreEqual(640, img.width);
                Assert.AreEqual(640, img.height);
            }
        }

        [TestMethod]
        public async Task GetCubeMap()
        {
            var cubeMapSearch = new GoogleMaps.CubeMapSearch("Washington, DC", 640, 640);
            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            var images = await gmaps.Get(cubeMapSearch);
            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.width);
                Assert.AreEqual(640, image.height);
            }
        }

        [TestMethod]
        public void GetCubeMapWithFlip_10x()
        {
            var cubeMapSearch = new GoogleMaps.CubeMapSearch("Washington, DC", 640, 640);
            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            var tasks = new Task<RawImage[]>[10];
            for (int i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = gmaps.Get(cubeMapSearch, true);
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
