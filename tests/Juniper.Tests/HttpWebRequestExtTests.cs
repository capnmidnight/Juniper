using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP.REST;
using Juniper.Imaging;
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
            await RunFileTest("portrait-testgetting.jpg", true, true).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TestCaching()
        {
            await RunFileTest("portrait-testcaching.jpg", true, false).ConfigureAwait(false);
            await RunFileTest("portrait-testcaching.jpg", false, true).ConfigureAwait(false);
        }

        private class ImageRequest : AbstractRequest<MediaType.Image>
        {
            public ImageRequest(Uri baseURI, string path)
                : base(HttpMethods.GET, AddPath(baseURI, path), Juniper.MediaType.Image.Jpeg) { }
        }

        private static async Task<ImageData> RunFileTest(string imageFileName, bool deleteFile, bool runTest)
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName);
            var cacheFile = new FileInfo(cacheFileName);

            if (deleteFile && cacheFile.Exists)
            {
                cacheFile.Delete();
            }

            var fileCache = new FileCacheLayer(cacheFile.Directory);
            var cache = new CachingStrategy()
                .AppendLayer(fileCache);

            var imageRequest = new ImageRequest(
                    new Uri("https://www.seanmcbeth.com"),
                    "2015-05.min.jpg");

            var imageDecoder = new TranscoderCodec<BitMiracle.LibJpeg.JpegImage, ImageData>(
                new LibJpegNETCodec(80),
                new LibJpegNETImageDataTranscoder());

            var image = await cache
                .Load(imageDecoder, imageRequest)
                .ConfigureAwait(false);

            if (runTest)
            {
                var path = Path.Combine(myPictures, "portrait-expected.jpg");
                var expected = imageDecoder.Deserialize(path);
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