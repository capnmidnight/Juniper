using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.Google.Maps.Tests;
using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Google.Maps.Geocoding.Tests
{
    [TestClass]
    public class GoogleGeocodingTests : ServicesTests
    {
        private static readonly IDeserializer<GeocodingResponse> decoder = new JsonFactory<GeocodingResponse>();
        [TestMethod]
        public async Task BasicGeocoding()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = "4909 Rutland Place, Alexandria, VA 22304"
            };
            var results = await cache.Decode(search, decoder);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);
        }

        [TestMethod]
        public async Task BasicComponentFilter()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = "High St, Hastings",
                CountryFilter = "GB"
            };
            var results = await cache.Decode(search, decoder);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);

            var res = results.results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("High St, Hastings TN34, UK", res.formatted_address);
        }

        [TestMethod]
        public async Task BasicGeocoding_WithAddressType()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            Assert.IsNotNull(results);
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);
        }

        [TestMethod]
        public async Task FormattedAddress()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("4909 Rutland Pl, Alexandria, VA 22304, USA", res.formatted_address);
        }

        [TestMethod]
        public async Task AddressType()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var type = res.types.FirstOrDefault();
            Assert.AreEqual(AddressComponentType.premise, type);
        }

        [TestMethod]
        public async Task GeomLocationType()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            Assert.AreEqual(GeometryLocationType.ROOFTOP, res.geometry.location_type);
        }

        [TestMethod]
        public async Task StreetNumber()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var streetNumber = res.GetAddressComponent(AddressComponentType.street_number);
            Assert.IsNotNull(streetNumber);
            Assert.AreEqual("4909", streetNumber.long_name);
            Assert.AreEqual("4909", streetNumber.short_name);
        }

        [TestMethod]
        public async Task Route()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var route = res.GetAddressComponent(AddressComponentType.route);
            Assert.IsNotNull(route);
            Assert.AreEqual("Rutland Place", route.long_name);
            Assert.AreEqual("Rutland Pl", route.short_name);
        }

        [TestMethod]
        public async Task PostalCode()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var zip = res.GetAddressComponent(AddressComponentType.postal_code);
            Assert.IsNotNull(zip);
            Assert.AreEqual("22304", zip.long_name);
            Assert.AreEqual("22304", zip.short_name);
        }

        [TestMethod]
        public async Task PostalCodeSuffix()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var zipSuffix = res.GetAddressComponent(AddressComponentType.postal_code_suffix);
            Assert.IsNotNull(zipSuffix);
            Assert.AreEqual("2111", zipSuffix.long_name);
            Assert.AreEqual("2111", zipSuffix.short_name);
        }

        [TestMethod]
        public async Task Neighborhood()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var neighborhood = res.GetAddressComponent(AddressComponentType.neighborhood, AddressComponentType.political);
            var altNeighborhood = res.GetAddressComponent(AddressComponentType.political, AddressComponentType.neighborhood);
            var simpNeighborhood = res.GetAddressComponent(AddressComponentType.neighborhood);
            Assert.IsNotNull(neighborhood);
            Assert.AreSame(neighborhood, altNeighborhood);
            Assert.AreSame(neighborhood, simpNeighborhood);
            Assert.AreEqual("Seminary Hill", neighborhood.long_name);
            Assert.AreEqual("Seminary Hill", neighborhood.short_name);
        }

        [TestMethod]
        public async Task City()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var locality = res.GetAddressComponent(AddressComponentType.locality, AddressComponentType.political);
            var altLocality = res.GetAddressComponent(AddressComponentType.political, AddressComponentType.locality);
            var simpLocality = res.GetAddressComponent(AddressComponentType.locality);
            Assert.IsNotNull(locality);
            Assert.AreSame(locality, altLocality);
            Assert.AreSame(locality, simpLocality);
            Assert.AreEqual("Alexandria", locality.long_name);
            Assert.AreEqual("Alexandria", locality.short_name);
        }

        [TestMethod]
        public async Task State()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var state = res.GetAddressComponent(AddressComponentType.administrative_area_level_1, AddressComponentType.political);
            var altState = res.GetAddressComponent(AddressComponentType.political, AddressComponentType.administrative_area_level_1);
            var simpState = res.GetAddressComponent(AddressComponentType.administrative_area_level_1);
            Assert.IsNotNull(state);
            Assert.AreSame(state, altState);
            Assert.AreSame(state, simpState);
            Assert.AreEqual("Virginia", state.long_name);
            Assert.AreEqual("VA", state.short_name);
        }

        [TestMethod]
        public async Task Country()
        {
            var search = new GeocodingRequest(apiKey)
            {
                Address = new USAddress(
                    "4909 Rutland Place",
                    "Alexandria", "VA", "22304")
                .ToString()
            };
            var results = await cache.Decode(search, decoder);
            var res = results.results.FirstOrDefault();
            var country = res.GetAddressComponent(AddressComponentType.country, AddressComponentType.political);
            var altCountry = res.GetAddressComponent(AddressComponentType.political, AddressComponentType.country);
            var simpCountry = res.GetAddressComponent(AddressComponentType.country);
            Assert.IsNotNull(country);
            Assert.AreSame(country, altCountry);
            Assert.AreSame(country, simpCountry);
            Assert.AreEqual("United States", country.long_name);
            Assert.AreEqual("US", country.short_name);
        }
    }
}