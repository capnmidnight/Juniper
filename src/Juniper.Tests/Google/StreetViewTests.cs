using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public class GoogleMapsCachingStrategy : CachingStrategy
        {
            public GoogleMapsCachingStrategy(string baseCachePath)
            {
                var gmapsCacheDirName = Path.Combine(baseCachePath, "GoogleMaps");
                var gmapsCacheDir = new DirectoryInfo(gmapsCacheDirName);
                AppendLayer(new GoogleMapsCacheLayer(gmapsCacheDir));
                AppendLayer(new GoogleMapsStreamingAssetsCacheLayer());
            }
        }
        public class StreamingAssetsCacheLayer : FileCacheLayer
        {
            private static string RootDir => @"C:\Users\smcbeth.DLS-INC\Projects\Yarrow\shared\StreamingAssets";

            public StreamingAssetsCacheLayer()
                : base(RootDir)
            {
            }
        }

        private Dictionary<string, MetadataResponse> metadataCache = new Dictionary<string, MetadataResponse>();

        private void Cache(MetadataResponse metadata)
        {
            metadataCache[metadata.Location.ToString(CultureInfo.InvariantCulture)] = metadata;
            metadataCache[metadata.Pano_ID] = metadata;
        }

        public class GoogleMapsStreamingAssetsCacheLayer : StreamingAssetsCacheLayer
        {
            public override bool CanCache(ContentReference fileRef)
            {
                return !(fileRef is IGoogleMapsRequest)
                    && base.CanCache(fileRef);
            }
        }

        [Test]
        public void GetAllManifests()
        {
            var baseCachePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cache = new GoogleMapsCachingStrategy(baseCachePath);
            var metadataDecoder = new JsonFactory<MetadataResponse>();
            var geocodingDecoder = new JsonFactory<GeocodingResponse>();

            var gmaps = new GoogleMapsClient(apiKey, signingKey, metadataDecoder, geocodingDecoder, cache);

            foreach (var (fileRef, metadata) in gmaps.CachedMetadata)
            {
                if (metadata.Location != null)
                {
                    Cache(metadata);
                }
                else
                {
                    cache.Delete(fileRef);
                }
            }
        }
    }
}