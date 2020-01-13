using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.World.GIS.Google.Tests;

using NUnit.Framework;

namespace Juniper.World.GIS.Google.Geocoding.Tests
{
    [TestFixture]
    public class GoogleReverseGeocodingTests : ServicesTests
    {
        [Test]
        public async Task BasicReverseGeocodingAsync()
        {
            var searchDecoder = new JsonFactory<GeocodingResponse>();
            var searchRequest = new ReverseGeocodingRequest(apiKey)
            {
                Location = new LatLngPoint(36.080811f, -75.721568f)
            };
            var results = await cache
                .LoadAsync(searchDecoder, searchRequest)
                .ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.Status);
            Assert.IsNull(results.Error_Message);
            Assert.IsNotNull(results.Results);

            var res = results.Results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("4877 The Woods Rd, Kitty Hawk, NC 27949, USA", res.Formatted_Address);
        }
    }
}