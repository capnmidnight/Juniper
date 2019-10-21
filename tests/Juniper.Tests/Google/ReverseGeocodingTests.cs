using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.World.GIS.Google.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.World.GIS.Google.Geocoding.Tests
{
    [TestClass]
    public class GoogleReverseGeocodingTests : ServicesTests
    {
        [TestMethod]
        public async Task BasicReverseGeocoding()
        {
            var searchDecoder = new JsonFactory<GeocodingResponse>();
            var searchRequest = new ReverseGeocodingRequest(apiKey)
            {
                Location = new LatLngPoint(36.080811f, -75.721568f)
            };
            var results = await cache.Load(searchDecoder, searchRequest);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);

            var res = results.results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("4877 The Woods Rd, Kitty Hawk, NC 27949, USA", res.formatted_address);
        }
    }
}