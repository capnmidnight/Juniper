using System;
using System.Text;
using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.World.GIS.Google.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.World.GIS.Google.MapTiles.Tests
{
    [TestClass]
    public class MapTilesTests : ServicesTests
    {
        [TestMethod]
        public void EncodeOnePart()
        {
            var sb = new StringBuilder();
            var first = 0;
            LinePathCollection.EncodePolylinePart(sb, -179.9832104, ref first);
            var encoded = sb.ToString();
            Assert.AreEqual("`~oia@", encoded);
        }

        [TestMethod]
        public void EncodePair()
        {
            EncodePolylinePartTest(
                "_p~iF~ps|U",
                "38.5, -120.2");
        }

        [TestMethod]
        public void EncodeString()
        {
            EncodePolylinePartTest(
                "_p~iF~ps|U_ulLnnqC_mqNvxq`@",
                "38.5, -120.2|40.7, -120.95|43.252, -126.453");
        }

        private static void EncodePolylinePartTest(string expected, string input)
        {
            var parts = input.SplitX('|');
            var encoded = LinePathCollection.EncodePolyline(parts);
            Assert.AreEqual(expected, encoded);
        }

        [TestMethod]
        public async Task GetImage()
        {
            var search = new TileRequest(apiKey, signingKey, new Size(640, 640))
            {
                Zoom = 20,
                Address = "4909 Rutland Pl, Alexandria, VA, 22304"
            };
            var decoder = new TranscoderCodec<Hjg.Pngcs.ImageLines, ImageData>(
                new HjgPngcsCodec(),
                new HjgPngcsImageDataTranscoder());
            var results = await cache
                .Load(decoder, search)
                .ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreEqual(640, results.info.dimensions.width);
            Assert.AreEqual(640, results.info.dimensions.height);
        }
    }
}