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
            private class ImageRequestConfiguration : AbstractRequestConfiguration
            {
                public ImageRequestConfiguration(DirectoryInfo cacheDir)
                    : base(new Uri("https://www.seanmcbeth.com"), cacheDir) { }
            }

            public ImageRequest(DirectoryInfo cacheDir)
                : base(new ImageRequestConfiguration(cacheDir), "2015-05.min.jpg") { }
        }

        private static async Task<ImageData> RunFileTest(string imageFileName, bool deleteFile, bool runTest)
        {
            var decoder = new LibJpegNETDecoder(80);
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName);
            var cacheFile = new FileInfo(cacheFileName);

            if (deleteFile && cacheFile.Exists)
            {
                cacheFile.Delete();
            }

            var actual = await new ImageRequest(cacheFile.Directory).GetDecoded(decoder);

            if (runTest)
            {
                var path = Path.Combine(myPictures, "portrait-expected.jpg");
                var expected = decoder.Load(path);
                Assert.AreEqual(expected.info.dimensions.width, actual.info.dimensions.width);
                Assert.AreEqual(expected.info.dimensions.height, actual.info.dimensions.height);
                Assert.AreEqual(expected.data.Length, actual.data.Length);
                for (var i = 0; i < expected.data.Length; ++i)
                {
                    Assert.AreEqual(expected.data[i], actual.data[i]);
                }
            }

            return actual;
        }
    }
}