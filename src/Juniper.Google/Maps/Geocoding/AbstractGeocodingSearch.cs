using System;
using System.IO;
using Juniper.HTTP.REST;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingSearch : AbstractMapsSearch<GeocodingResults>
    {
        protected AbstractGeocodingSearch()
            : base("geocode/json", "geocoding", "application/json", "json", false)
        {
        }

        protected AbstractGeocodingSearch(string paramName, string paramValue)
            : this()
        {
            SetQuery(paramName, paramValue);
        }

        public override Func<Stream, GeocodingResults> GetDecoder(AbstractEndpoint api)
        {
            return api.DecodeObject<GeocodingResults>;
        }

        public void SetLanguage(string language)
        {
            SetQuery(nameof(language), language);
        }
    }
}