using System;
using System.IO;
using System.Threading.Tasks;
using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.Imaging.LibJpegNET;
using Juniper.IO;

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

        private class ImageRequest : AbstractRequest<MediaType.Image>
        {
            public ImageRequest(Uri baseURI, string path)
                : base(AddPath(baseURI, path), MediaType.Image.Jpeg) { }

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
            var imageDecoder = new LibJpegNETImageDataTranscoder(80);
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName);
            var cacheFile = new FileInfo(cacheFileName);

            if (deleteFile && cacheFile.Exists)
            {
                cacheFile.Delete();
            }

            var fileCache = new FileCacheLayer(cacheFile.Directory);
            var cache = new CachingStrategy()
                .AddLayer(fileCache);

            var imageRequest = new ImageRequest(
                    new Uri("https://www.seanmcbeth.com"),
                    "2015-05.min.jpg");

            var image = await cache
                .GetStreamSource(imageRequest)
                .Decode(imageDecoder);

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