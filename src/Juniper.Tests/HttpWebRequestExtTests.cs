using System;
using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP.Client.REST;
using Juniper.Imaging;
using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.HTTP.Tests
{
    [TestClass]
    public class HttpWebRequestExtTests
    {
        [TestMethod]
        public async Task TestGettingAsync()
        {
            await RunFileTestAsync("portrait-testgetting.jpg", true, true).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TestCachingAsync()
        {
            await RunFileTestAsync("portrait-testcaching.jpg", true, false).ConfigureAwait(false);
            await RunFileTestAsync("portrait-testcaching.jpg", false, true).ConfigureAwait(false);
        }

        private class ImageRequest : AbstractRequest<MediaType.Image>
        {
            public ImageRequest(Uri baseURI, string path)
                : base(HttpMethods.GET, AddPath(baseURI, path), Juniper.MediaType.Image.Jpeg, false) { }
        }

        private static async Task<ImageData> RunFileTestAsync(string imageFileName, bool deleteFile, bool runTest)
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheFileName = Path.Combine(myPictures, imageFileName.ValidateFileName());
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
                .LoadAsync(imageDecoder, imageRequest)
                .ConfigureAwait(false);

            if (runTest)
            {
                var path = Path.Combine(myPictures, "portrait-expected.jpg");
                var expected = imageDecoder.Deserialize(path);
                Assert.AreEqual(expected.Info.Dimensions.Width, image.Info.Dimensions.Width);
                Assert.AreEqual(expected.Info.Dimensions.Height, image.Info.Dimensions.Height);
                Assert.AreEqual(expected.Data.Length, image.Data.Length);
                for (var i = 0; i < expected.Data.Length; ++i)
                {
                    Assert.AreEqual(expected.Data[i], image.Data[i]);
                }
            }

            return image;
        }
    }
}