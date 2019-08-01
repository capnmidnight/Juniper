using Juniper.HTTP.REST;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractMapsRequest<GeocodingResponse>
    {
        protected AbstractGeocodingRequest(AbstractEndpoint api)
            : base(api, new Json.Factory<GeocodingResponse>(), "geocode/json", "geocoding", false)
        {
            SetContentType("application/json", "json");
        }

        protected AbstractGeocodingRequest(AbstractEndpoint api, string paramName, string paramValue)
            : this(api)
        {
            SetQuery(paramName, paramValue);
        }

        public void SetLanguage(string language)
        {
            SetQuery(nameof(language), language);
        }
    }
}