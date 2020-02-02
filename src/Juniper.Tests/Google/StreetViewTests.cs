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
        private IImageCodec<ImageData> jpegDecoder;
        private IImageCodec<ImageData> pngDecoder;

        [SetUp]
        public override void Init()
        {
            base.Init();

            jpegDecoder = new TranscoderCodec<BitMiracle.LibJpeg.JpegImage, ImageData>(
                new LibJpegNETCodec(80),
                new LibJpegNETImageDataTranscoder());

            pngDecoder = new TranscoderCodec<Hjg.Pngcs.ImageLines, ImageData>(
                new HjgPngcsCodec(),
                new HjgPngcsImageDataTranscoder());
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
            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var metadataRequest = new MetadataRequest(apiKey, signingKey)
            {
                Place = "Washington, DC"
            };
            var metadata = await cache
                .LoadAsync(metadataDecoder, metadataRequest)
                .ConfigureAwait(false);
            Assert.AreEqual(HttpStatusCode.OK, metadata.Status);
            Assert.IsNotNull(metadata.Copyright);
            Assert.AreEqual("2016-07", metadata.Date.ToString("yyyy-MM", CultureInfo.InvariantCulture));
            Assert.IsNotNull(metadata.Location);
            Assert.IsNotNull(metadata.Pano_ID);
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

        public class GoogleMapsStreamingAssetsCacheLayer : FileCacheLayer
        {
            public GoogleMapsStreamingAssetsCacheLayer(string unityProjectDir)
                : base(Path.Combine(unityProjectDir, "Assets", "StreamingAssets", "Google", "StreetView"))
            { }

            public GoogleMapsStreamingAssetsCacheLayer(string unityProjectDir, string prefix)
                : base(Path.Combine(unityProjectDir, "Assets", "StreamingAssets", prefix, "Google", "StreetView"))
            { }

            public override bool CanCache(ContentReference fileRef)
            {
                return !(fileRef is IGoogleMapsRequest)
                    && base.CanCache(fileRef);
            }
        }

        [Test]
        public void GetAllMetadata()
        {
            var unityProjectDir = @"C:\Users\smcbeth.DLS-INC\Projects\Yarrow\src\Yarrow - AndroidOculus";
            var cache = new CachingStrategy
            {
                new GoogleMapsStreamingAssetsCacheLayer(unityProjectDir, "Yarrow")
            };

            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var geocodingDecoder = new JsonFactory<GeocodingResponse>();

            var gmaps = new GoogleMapsClient<MetadataResponse>(apiKey, signingKey, metadataDecoder, geocodingDecoder, cache);
            var files = gmaps.CachedMetadata.ToArray();
            Assert.AreNotEqual(0, files.Length);
        }

        [Test]
        public async Task GetMetadataByFileRefAsync()
        {
            var unityProjectDir = @"C:\Users\smcbeth.DLS-INC\Projects\Yarrow\src\Yarrow - AndroidOculus";
            var cache = new CachingStrategy
            {
                new GoogleMapsStreamingAssetsCacheLayer(unityProjectDir, "Yarrow")
            };

            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var geocodingDecoder = new JsonFactory<GeocodingResponse>();

            var gmaps = new GoogleMapsClient<MetadataResponse>(apiKey, signingKey, metadataDecoder, geocodingDecoder, cache);
            var (_, metadata) = gmaps.CachedMetadata.FirstOrDefault();
            var pano = metadata.Pano_ID;
            var fileRef = new ContentReference(pano, MediaType.Application.Json);
            var metadata2 = await cache.LoadAsync(metadataDecoder, fileRef)
                .ConfigureAwait(false);
            Assert.AreEqual(metadata.Pano_ID, metadata2.Pano_ID);
        }
    }
}