using System;
using System.IO;

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

        internal override Func<Stream, GeocodingResults> GetDecoder(AbstractAPI api)
        {
            return api.DecodeObject<GeocodingResults>;
        }

        public void SetLanguage(string regionCode)
        {
            SetQuery("language", regionCode);
        }
    }
}