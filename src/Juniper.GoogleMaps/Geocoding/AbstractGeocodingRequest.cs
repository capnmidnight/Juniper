using System.Net.Http;

namespace Juniper.World.GIS.Google.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractGoogleMapsRequest<MediaType.Application>
    {
        private string language;

        protected AbstractGeocodingRequest(HttpClient http, string apiKey)
            : base(http, "geocode/json", Juniper.MediaType.Application_Json, apiKey, null)
        {
        }

        public string Language
        {
            get => language;
            set
            {
                language = value;
                SetQuery(nameof(language), language);
            }
        }
    }
}