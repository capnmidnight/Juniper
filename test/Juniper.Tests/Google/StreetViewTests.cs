using Juniper.Caching;
using Juniper.Imaging;
using Juniper.IO;
using Juniper.World.GIS.Google.Geocoding;

using NUnit.Framework;

using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Juniper.World.GIS.Google.StreetView
{
    [TestFixture]
    public class StreetViewTests : ServicesTests
    {
        private HttpClient http;

        [SetUp]
        public void Setup()
        {
            http = new(new HttpClientHandler
            {
                UseCookies = false
            });
        }

        [TearDown]
        public void TearDown()
        {
            http?.Dispose();
            http = null;
        }

        private IImageFactory<ImageData> jpegDecoder;
        private IImageFactory<ImageData> pngDecoder;

        private JsonFactory<MetadataResponse> metadataDecoder;

        [SetUp]
        public override void Init()
        {
            base.Init();

            jpegDecoder = new JpegFactory(80).Pipe(new JpegCodec());
            pngDecoder = new PngFactory().Pipe(new PngCodec());

            metadataDecoder = new JsonFactory<MetadataResponse>();
        }

        [Test]
        public async Task JPEGImageSizeAsync()
        {
            var imageRequest = new ImageRequest(http, apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await cache
                .LoadAsync(jpegDecoder, imageRequest)
                .ConfigureAwait(false);
            var info = image.Info;
            Assert.AreEqual(640, info.Dimensions.Width);
            Assert.AreEqual(640, info.Dimensions.Height);
        }

        [Test]
        public async Task PNGImageSizeAsync()
        {
            var imageRequest = new ImageRequest(http, apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var rawImg = await cache
                .LoadAsync(jpegDecoder, imageRequest)
                .ConfigureAwait(false);
            var data = pngDecoder.Serialize(rawImg);
            var info = ImageInfo.ReadPNG(data);
            Assert.AreEqual(640, info.Dimensions.Width);
            Assert.AreEqual(640, info.Dimensions.Height);
        }

        [Test]
        public async Task GetMetadataAsync()
        {
            var metadataRequest = new MetadataRequest(http, apiKey, signingKey)
            {
                Place = "Washington, DC"
            };
            var metadata = await cache
                .LoadAsync(metadataDecoder, metadataRequest)
                .ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, metadata.Status);
            Assert.IsNotNull(metadata.Copyright);
            Assert.AreEqual("2016-07", metadata.Date?.ToString("yyyy-MM", CultureInfo.InvariantCulture));
            Assert.IsNotNull(metadata.Location);
            Assert.IsNotNull(metadata.Pano_id);
        }

        [Test]
        public async Task GetImageAsync()
        {
            var imageRequest = new ImageRequest(http, apiKey, signingKey, new Size(4096, 4096))
            {
                Place = "Alexandria, VA"
            };

            var image = await cache
                .LoadAsync(jpegDecoder, imageRequest)
                .ConfigureAwait(false);
            Assert.AreEqual(640, image.Info.Dimensions.Width);
            Assert.AreEqual(640, image.Info.Dimensions.Height);
        }

        [Test]
        public async Task GetImageWithoutCachingAsync()
        {
            var imageRequest = new ImageRequest(http, apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest
                .DecodeAsync(jpegDecoder)
                .ConfigureAwait(false);
            Assert.AreEqual(640, image.Info.Dimensions.Width);
            Assert.AreEqual(640, image.Info.Dimensions.Height);
        }
    }
}