using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.World.GIS.Google.Tests;

using NUnit.Framework;

namespace Juniper.World.GIS.Google.Geocoding.Tests
{
    [TestFixture]
    public class GoogleGeocodingTests : ServicesTests
    {
        private static readonly IJsonDecoder<GeocodingResponse> decoder = new JsonFactory<GeocodingResponse>();

        [Test]
        public async Task BasicGeocodingAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = "4909 Rutland Place, Alexandria, VA 22304"
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.Status);
            Assert.IsNull(results.Error_Message);
            Assert.IsNotNull(results.Results);
        }

        [Test]
        public async Task BasicComponentFilterAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = "High St, Hastings",
                CountryFilter = "GB"
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.Status);
            Assert.IsNull(results.Error_Message);
            Assert.IsNotNull(results.Results);

            var res = results.Results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("High St, Hastings TN34, UK", res.Formatted_Address);
        }

        [Test]
        public async Task BasicGeocoding_WithAddressTypeAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.Status);
            Assert.IsNull(results.Error_Message);
            Assert.IsNotNull(results.Results);
        }

        [Test]
        public async Task FormattedAddressAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("4909 Rutland Pl, Alexandria, VA 22304, USA", res.Formatted_Address);
        }

        [Test]
        public async Task AddressTypeAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var type = res.Types.FirstOrDefault();
            Assert.AreEqual(AddressComponentTypes.premise, type);
        }

        [Test]
        public async Task GeomLocationTypeAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            Assert.AreEqual(GeometryLocationType.ROOFTOP, res.Geometry.Location_Type);
        }

        [Test]
        public async Task StreetNumberAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var streetNumber = res.GetAddressComponent(AddressComponentTypes.street_number);
            Assert.IsNotNull(streetNumber);
            Assert.AreEqual("4909", streetNumber.Long_Name);
            Assert.AreEqual("4909", streetNumber.Short_Name);
        }

        [Test]
        public async Task RouteAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var route = res.GetAddressComponent(AddressComponentTypes.route);
            Assert.IsNotNull(route);
            Assert.AreEqual("Rutland Place", route.Long_Name);
            Assert.AreEqual("Rutland Pl", route.Short_Name);
        }

        [Test]
        public async Task PostalCodeAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var zip = res.GetAddressComponent(AddressComponentTypes.postal_code);
            Assert.IsNotNull(zip);
            Assert.AreEqual("22304", zip.Long_Name);
            Assert.AreEqual("22304", zip.Short_Name);
        }

        [Test]
        public async Task PostalCodeSuffixAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var zipSuffix = res.GetAddressComponent(AddressComponentTypes.postal_code_suffix);
            Assert.IsNotNull(zipSuffix);
            Assert.AreEqual("2111", zipSuffix.Long_Name);
            Assert.AreEqual("2111", zipSuffix.Short_Name);
        }

        [Test]
        public async Task NeighborhoodAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var neighborhood = res.GetAddressComponent(AddressComponentTypes.neighborhood, AddressComponentTypes.political);
            var altNeighborhood = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.neighborhood);
            var simpNeighborhood = res.GetAddressComponent(AddressComponentTypes.neighborhood);
            Assert.IsNotNull(neighborhood);
            Assert.AreSame(neighborhood, altNeighborhood);
            Assert.AreSame(neighborhood, simpNeighborhood);
            Assert.AreEqual("Seminary Hill", neighborhood.Long_Name);
            Assert.AreEqual("Seminary Hill", neighborhood.Short_Name);
        }

        [Test]
        public async Task CityAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var locality = res.GetAddressComponent(AddressComponentTypes.locality, AddressComponentTypes.political);
            var altLocality = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.locality);
            var simpLocality = res.GetAddressComponent(AddressComponentTypes.locality);
            Assert.IsNotNull(locality);
            Assert.AreSame(locality, altLocality);
            Assert.AreSame(locality, simpLocality);
            Assert.AreEqual("Alexandria", locality.Long_Name);
            Assert.AreEqual("Alexandria", locality.Short_Name);
        }

        [Test]
        public async Task StateAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var state = res.GetAddressComponent(AddressComponentTypes.administrative_area_level_1, AddressComponentTypes.political);
            var altState = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.administrative_area_level_1);
            var simpState = res.GetAddressComponent(AddressComponentTypes.administrative_area_level_1);
            Assert.IsNotNull(state);
            Assert.AreSame(state, altState);
            Assert.AreSame(state, simpState);
            Assert.AreEqual("Virginia", state.Long_Name);
            Assert.AreEqual("VA", state.Short_Name);
        }

        [Test]
        public async Task CountryAsync()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache
                .LoadAsync(decoder, search)
                .ConfigureAwait(false);
            var res = results.Results.FirstOrDefault();
            var country = res.GetAddressComponent(AddressComponentTypes.country, AddressComponentTypes.political);
            var altCountry = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.country);
            var simpCountry = res.GetAddressComponent(AddressComponentTypes.country);
            Assert.IsNotNull(country);
            Assert.AreSame(country, altCountry);
            Assert.AreSame(country, simpCountry);
            Assert.AreEqual("United States", country.Long_Name);
            Assert.AreEqual("US", country.Short_Name);
        }
    }
}