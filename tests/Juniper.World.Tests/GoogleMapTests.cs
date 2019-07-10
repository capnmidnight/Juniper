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
        public async Task GetImageFromPanoID()
        {
            var metadataSearch = new GoogleMaps.MetadataSearch("Washington, DC");
            var metadata = await gmaps.Get(metadataSearch);
            var imageSearch = new GoogleMaps.ImageSearch(metadata.pano_id, 640, 640);
            var image = await gmaps.Get(imageSearch);
            Assert.IsTrue(gmaps.IsCached(imageSearch));
            Assert.AreEqual(640, image.width);
            Assert.AreEqual(640, image.height);
        }

        [TestMethod]
        public async Task GetCubeMapFromPanoID()
        {
            var metadataSearch = new GoogleMaps.MetadataSearch("Washington, DC");
            var metadata = await gmaps.Get(metadataSearch);
            var cubeMapSearch = new GoogleMaps.CubeMapSearch(metadata.pano_id, 640, 640);
            var images = await gmaps.Get(cubeMapSearch);
            Assert.IsTrue(gmaps.IsCached(cubeMapSearch));
            foreach (var image in images)
            {
                Assert.AreEqual(640, image.width);
                Assert.AreEqual(640, image.height);
            }
        }
    }
}
