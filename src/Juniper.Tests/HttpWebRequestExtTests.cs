using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.IO;

using NUnit.Framework;

namespace Juniper.HTTP.Tests
{
    [TestFixture]
    public class HttpWebRequestExtTests
    {
        [Test]
        public async Task TestGettingAsync()
        {
            await RunFileTestAsync("portrait-testgetting.jpg", true, true).ConfigureAwait(false);
        }

        [Test]
        public async Task TestCachingAsync()
        {
            await RunFileTestAsync("portrait-testcaching.jpg", true, false).ConfigureAwait(false);
            await RunFileTestAsync("portrait-testcaching.jpg", false, true).ConfigureAwait(false);
        }

        private sealed class ImageRequest : AbstractRequest<MediaType.Image>
        {
            public ImageRequest(Uri baseURI, string path)
                : base(HttpMethods.GET, AddPath(baseURI, path), Juniper.MediaType.Image.Jpeg, false) { }

            protected override string InternalCacheID =>
                StandardRequestCacheID;
        }

        private static async Task<ImageData> RunFileTestAsync(string imageFileName, bool deleteFile, bool runTest)
        {
            if (imageFileName is null)
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            if (imageFileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(imageFileName));
            }

            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName);
            var cacheFile = new FileInfo(cacheFileName);

            if (deleteFile && cacheFile.Exists)
            {
                cacheFile.Delete();
            }

            var cache = new CachingStrategy
            {
                new FileCacheLayer(cacheFile.Directory)
            };

            var imageRequest = new ImageRequest(
                    new Uri("https://www.seanmcbeth.com"),
                    "2015-05.min.jpg");

            var imageDecoder = new TranscoderCodec<BitMiracle.LibJpeg.JpegImage, ImageData>(
                new LibJpegNETCodec(80),
                new LibJpegNETImageDataTranscoder());

            var actual = await cache
                .LoadAsync(imageDecoder, imageRequest)
                .ConfigureAwait(false);

            if (runTest)
            {
                var path = Path.Combine(myPictures, "portrait-expected.jpg");
                var expected = imageDecoder.Deserialize(path);
                var expectedData = expected.GetData();
                var actualData = actual.GetData();
                Assert.AreEqual(expected.Info.Dimensions.Width, actual.Info.Dimensions.Width);
                Assert.AreEqual(expected.Info.Dimensions.Height, actual.Info.Dimensions.Height);
                Assert.AreEqual(expectedData.Length, actualData.Length);
                for (var i = 0; i < expectedData.Length; ++i)
                {
                    Assert.AreEqual(expectedData[i], actualData[i]);
                }
            }

            return actual;
        }
    }
}