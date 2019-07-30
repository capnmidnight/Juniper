using System;
using System.IO;

using Juniper.HTTP.REST;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractMapsRequest<GeocodingResponse>
    {
        protected AbstractGeocodingRequest()
            : base(new Json.Factory<GeocodingResponse>(), "geocode/json", "geocoding", false)
        {
            SetContentType("application/json", "json");
        }

        protected AbstractGeocodingRequest(string paramName, string paramValue)
            : this()
        {
            SetQuery(paramName, paramValue);
        }

        public void SetLanguage(string language)
        {
            SetQuery(nameof(language), language);
        }
    }
}