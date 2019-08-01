using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Image;
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
            await RunFileTest(true, true, ImageSource.Network);
        }

        [TestMethod]
        public async Task TestCaching()
        {
            await RunFileTest(true, false, ImageSource.Network);
            await RunFileTest(false, true, ImageSource.File);
        }

        private static async Task<ImageData> RunFileTest(bool deleteFile, bool runTest, ImageSource expectedSource)
        {
            var decoder = new Image.JPEG.Factory();
            const string cacheFileName = "portrait.jpg";
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFile = Path.Combine(myPictures, cacheFileName);

            if (deleteFile && File.Exists(cacheFile))
            {
                File.Delete(cacheFile);
            }

            var actual = await HttpWebRequestExt.CachedGet(
                new Uri("https://www.seanmcbeth.com/2015-05.min.jpg"),
                decoder.Deserialize,
                Path.Combine(myPictures, cacheFile));

            Assert.AreEqual(expectedSource, actual.source);

            if (runTest)
            {
                var path = Path.Combine(myPictures, "portrait-expected.jpg");
                var expected = decoder.Deserialize(path);
                Assert.AreEqual(expected.dimensions.width, actual.dimensions.width);
                Assert.AreEqual(expected.dimensions.height, actual.dimensions.height);
                Assert.AreEqual(expected.data.Length, actual.data.Length);
                for (int i = 0; i < expected.data.Length; ++i)
                {
                    Assert.AreEqual(expected.data[i], actual.data[i]);
                }
            }

            return actual;
        }
    }
}