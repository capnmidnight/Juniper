using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.Imaging.LibJpegNET;
using Juniper.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.HTTP.Tests
{
    [TestClass]
    public class HttpWebRequestExtTests
    {
        [TestMethod]
        public async Task TestGetting()
        {
            await RunFileTest("portrait-testgetting.jpg", true, true);
        }

        [TestMethod]
        public async Task TestCaching()
        {
            await RunFileTest("portrait-testcaching.jpg", true, false);
            await RunFileTest("portrait-testcaching.jpg", false, true);
        }

        private class ImageRequest : AbstractRequest
        {
            public ImageRequest(Uri baseURI, string path, MediaType.Image imageType)
                : base(AddPath(baseURI, path), imageType) { }

            protected override ActionDelegate Action
            {
                get
                {
                    return Get;
                }
            }
        }

        private static async Task<ImageData> RunFileTest(string imageFileName, bool deleteFile, bool runTest)
        {
            var imageDecoder = new LibJpegNETDecoder(80);
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName);
            var cacheFile = new FileInfo(cacheFileName);

            if (deleteFile && cacheFile.Exists)
            {
                cacheFile.Delete();
            }

            var fileCache = new FileCacheLayer(cacheFile.Directory);

            var imageRequest = new ImageRequest(
                    new Uri("https://www.seanmcbeth.com"),
                    "2015-05.min.jpg",
                    MediaType.Image.Jpeg);

            var image = await fileCache.GetDecoded(imageRequest, imageDecoder);

            if (runTest)
            {
                var path = Path.Combine(myPictures, "portrait-expected.jpg");
                var expected = imageDecoder.Load(path);
                Assert.AreEqual(expected.info.dimensions.width, image.info.dimensions.width);
                Assert.AreEqual(expected.info.dimensions.height, image.info.dimensions.height);
                Assert.AreEqual(expected.data.Length, image.data.Length);
                for (var i = 0; i < expected.data.Length; ++i)
                {
                    Assert.AreEqual(expected.data[i], image.data[i]);
                }
            }

            return image;
        }
    }
}