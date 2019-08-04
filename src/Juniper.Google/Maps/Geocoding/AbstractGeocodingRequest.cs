using Juniper.Json;
using Juniper.Serialization;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractMapsRequest<GeocodingResponse>
    {
        protected AbstractGeocodingRequest(GoogleMapsRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<GeocodingResponse>(), "geocode/json", "geocoding", false)
        {
            SetContentType("application/json", "json");
        }

        protected AbstractGeocodingRequest(GoogleMapsRequestConfiguration api, string paramName, string paramValue)
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