using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Juniper.Image;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.HTTP.Tests
{
    [TestClass]
    public class HttpWebRequestExtTests
    {
        [TestMethod]
        public async Task TestGetting()
        {
            await RunFileTest(true, true, RawImage.ImageSource.Network);
        }

        [TestMethod]
        public async Task TestCaching()
        {
            await RunFileTest(true, false, RawImage.ImageSource.Network);
            await RunFileTest(false, true, RawImage.ImageSource.File);
        }

        private static async Task<RawImage> RunFileTest(bool deleteFile, bool runTest, RawImage.ImageSource expectedSource)
        {
            const string cacheFileName = "portrait.jpg";
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFile = Path.Combine(myPictures, cacheFileName);

            if (deleteFile && File.Exists(cacheFile))
            {
                File.Delete(cacheFile);
            }

            var actual = await HttpWebRequestExt.CachedGet(
                new Uri("https://www.seanmcbeth.com/2015-05.min.jpg"),
                stream => Decoder.DecodeJPEG(stream, false),
                Path.Combine(myPictures, cacheFile));

            Assert.AreEqual(expectedSource, actual.source);

            if (runTest)
            {
                var expected = Decoder.DecodeJPEG(File.ReadAllBytes(Path.Combine(myPictures, "portrait-expected.jpg")), false);
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