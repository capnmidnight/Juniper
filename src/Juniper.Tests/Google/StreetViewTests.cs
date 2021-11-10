using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.World.GIS.Google.Geocoding;
using Juniper.World.GIS.Google.Tests;

using NUnit.Framework;

namespace Juniper.World.GIS.Google.StreetView.Tests
{
    [TestFixture]
    public class StreetViewTests : ServicesTests
    {
        private const string UnityProjectDir = @"C:\Users\smcbeth.DLS-INC\Projects\Yarrow\src\Yarrow - AndroidOculus";
        private const string ProjectName = "Yarrow";
        private static readonly string UnityProjectStreamingAssetsDir = Path.Combine(UnityProjectDir, "Assets", "StreamingAssets");
        private static readonly string GmapsStreamingAssetsDir = Path.Combine(UnityProjectStreamingAssetsDir, ProjectName, "Google", "StreetView");

        private IImageCodec<ImageData> jpegDecoder;
        private IImageCodec<ImageData> pngDecoder;

        private JsonFactory<MetadataResponse> metadataDecoder;
        private JsonFactory<GeocodingResponse> geocodingDecoder;

        [SetUp]
        public override void Init()
        {
            base.Init();

            jpegDecoder = new TranscoderCodec<BitMiracle.LibJpeg.JpegImage, ImageData>(
                new JpegCodec(80),
                new JpegTranscoder());

            pngDecoder = new TranscoderCodec<Hjg.Pngcs.ImageLines, ImageData>(
                new PngCodec(),
                new PngTranscoder());

            metadataDecoder = new JsonFactory<MetadataResponse>();
            geocodingDecoder = new JsonFactory<GeocodingResponse>();
        }

        [Test]
        public async Task JPEGImageSizeAsync()
        {
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
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
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
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
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
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
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(4096, 4096))
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
            var imageRequest = new ImageRequest(apiKey, signingKey, new Size(640, 640))
            {
                Place = "Alexandria, VA"
            };

            var image = await imageRequest
                .DecodeAsync(jpegDecoder)
                .ConfigureAwait(false);
            Assert.AreEqual(640, image.Info.Dimensions.Width);
            Assert.AreEqual(640, image.Info.Dimensions.Height);
        }

        [Test]
        public void GetAllMetadata()
        {
            var cache = new CachingStrategy
            {
                new FileCacheLayer(GmapsStreamingAssetsDir)
            };

            var gmaps = new GoogleMapsClient(apiKey, signingKey, metadataDecoder, geocodingDecoder, cache);
            var files = gmaps.CachedMetadata.ToArray();
            Assert.AreNotEqual(0, files.Length);
        }

        [Test]
        public async Task GetMetadataByFileRefAsync()
        {
            var cache = new CachingStrategy
            {
                new FileCacheLayer(GmapsStreamingAssetsDir)
            };

            var gmaps = new GoogleMapsClient(apiKey, signingKey, metadataDecoder, geocodingDecoder, cache);
            var metadata = gmaps.CachedMetadata.FirstOrDefault();
            var pano = metadata.Pano_id;
            var fileRef = new ContentReference(pano, MediaType.Application.Json);
            var metadata2 = await cache.LoadAsync(metadataDecoder, fileRef)
                .ConfigureAwait(false);
            Assert.AreEqual(metadata.Pano_id, metadata2.Pano_id);
        }
    }
}