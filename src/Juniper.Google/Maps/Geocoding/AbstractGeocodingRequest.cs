using Juniper.Json;
using Juniper.Serialization;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractGoogleMapsRequest<IDeserializer<GeocodingResponse>, GeocodingResponse>
    {
        private string language;

        protected AbstractGeocodingRequest(GoogleMapsRequestConfiguration api)
            : base(api, new JsonFactory().Specialize<GeocodingResponse>(), "geocode/json", "geocoding", false)
        {
            SetContentType(HTTP.MediaType.Application.Json);
        }

        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                SetQuery(nameof(language), language);
            }
        }
    }
}