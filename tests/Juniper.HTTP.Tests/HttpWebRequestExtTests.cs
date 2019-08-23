using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

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
            await RunFileTest("portrait-testgetting.jpg", true, true, DataSource.Network);
        }

        [TestMethod]
        public async Task TestCaching()
        {
            await RunFileTest("portrait-testcaching.jpg", true, false, DataSource.Network);
            await RunFileTest("portrait-testcaching.jpg", false, true, DataSource.File);
        }

        private static async Task<ImageData> RunFileTest(string imageFileName, bool deleteFile, bool runTest, DataSource expectedSource)
        {
            var decoder = new LibJpegNETImageDataCodec();
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName);
            var cacheFile = new FileInfo(cacheFileName);

            if (deleteFile && cacheFile.Exists)
            {
                cacheFile.Delete();
            }

            var actual = await HttpWebRequestExt.CachedGet(
                new Uri("https://www.seanmcbeth.com/2015-05.min.jpg"),
                decoder.Deserialize,
                cacheFile);

            Assert.AreEqual(expectedSource, actual.info.source);

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