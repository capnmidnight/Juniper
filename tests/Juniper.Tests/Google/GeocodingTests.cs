using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.World.GIS.Google.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.World.GIS.Google.Geocoding.Tests
{
    [TestClass]
    public class GoogleGeocodingTests : ServicesTests
    {
        private static readonly IJsonDecoder<GeocodingResponse> decoder = new JsonFactory<GeocodingResponse>();

        [TestMethod]
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
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);
        }

        [TestMethod]
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
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);

            var res = results.results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("High St, Hastings TN34, UK", res.formatted_address);
        }

        [TestMethod]
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
            Assert.AreEqual(HttpStatusCode.OK, results.status);
            Assert.IsNull(results.error_message);
            Assert.IsNotNull(results.results);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            Assert.IsNotNull(res);
            Assert.AreEqual("4909 Rutland Pl, Alexandria, VA 22304, USA", res.formatted_address);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var type = res.types.FirstOrDefault();
            Assert.AreEqual(AddressComponentTypes.premise, type);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            Assert.AreEqual(GeometryLocationType.ROOFTOP, res.geometry.location_type);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var streetNumber = res.GetAddressComponent(AddressComponentTypes.street_number);
            Assert.IsNotNull(streetNumber);
            Assert.AreEqual("4909", streetNumber.long_name);
            Assert.AreEqual("4909", streetNumber.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var route = res.GetAddressComponent(AddressComponentTypes.route);
            Assert.IsNotNull(route);
            Assert.AreEqual("Rutland Place", route.long_name);
            Assert.AreEqual("Rutland Pl", route.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var zip = res.GetAddressComponent(AddressComponentTypes.postal_code);
            Assert.IsNotNull(zip);
            Assert.AreEqual("22304", zip.long_name);
            Assert.AreEqual("22304", zip.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var zipSuffix = res.GetAddressComponent(AddressComponentTypes.postal_code_suffix);
            Assert.IsNotNull(zipSuffix);
            Assert.AreEqual("2111", zipSuffix.long_name);
            Assert.AreEqual("2111", zipSuffix.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var neighborhood = res.GetAddressComponent(AddressComponentTypes.neighborhood, AddressComponentTypes.political);
            var altNeighborhood = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.neighborhood);
            var simpNeighborhood = res.GetAddressComponent(AddressComponentTypes.neighborhood);
            Assert.IsNotNull(neighborhood);
            Assert.AreSame(neighborhood, altNeighborhood);
            Assert.AreSame(neighborhood, simpNeighborhood);
            Assert.AreEqual("Seminary Hill", neighborhood.long_name);
            Assert.AreEqual("Seminary Hill", neighborhood.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var locality = res.GetAddressComponent(AddressComponentTypes.locality, AddressComponentTypes.political);
            var altLocality = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.locality);
            var simpLocality = res.GetAddressComponent(AddressComponentTypes.locality);
            Assert.IsNotNull(locality);
            Assert.AreSame(locality, altLocality);
            Assert.AreSame(locality, simpLocality);
            Assert.AreEqual("Alexandria", locality.long_name);
            Assert.AreEqual("Alexandria", locality.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var state = res.GetAddressComponent(AddressComponentTypes.administrative_area_level_1, AddressComponentTypes.political);
            var altState = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.administrative_area_level_1);
            var simpState = res.GetAddressComponent(AddressComponentTypes.administrative_area_level_1);
            Assert.IsNotNull(state);
            Assert.AreSame(state, altState);
            Assert.AreSame(state, simpState);
            Assert.AreEqual("Virginia", state.long_name);
            Assert.AreEqual("VA", state.short_name);
        }

        [TestMethod]
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
            var res = results.results.FirstOrDefault();
            var country = res.GetAddressComponent(AddressComponentTypes.country, AddressComponentTypes.political);
            var altCountry = res.GetAddressComponent(AddressComponentTypes.political, AddressComponentTypes.country);
            var simpCountry = res.GetAddressComponent(AddressComponentTypes.country);
            Assert.IsNotNull(country);
            Assert.AreSame(country, altCountry);
            Assert.AreSame(country, simpCountry);
            Assert.AreEqual("United States", country.long_name);
            Assert.AreEqual("US", country.short_name);
        }
    }
}