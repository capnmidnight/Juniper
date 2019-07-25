using System;
using System.IO;

using Juniper.HTTP.REST;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractMapsRequest<GeocodingResponse>
    {
        protected AbstractGeocodingRequest()
            : base("geocode/json", "geocoding", "application/json", "json", false)
        {
        }

        protected AbstractGeocodingRequest(string paramName, string paramValue)
            : this()
        {
            SetQuery(paramName, paramValue);
        }

        public override Func<Stream, GeocodingResponse> GetDecoder(AbstractEndpoint api)
        {
            return api.DecodeObject<GeocodingResponse>;
        }

        public void SetLanguage(string language)
        {
            SetQuery(nameof(language), language);
        }
    }
}