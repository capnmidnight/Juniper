namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractGoogleMapsRequest
    {
        private string language;

        protected AbstractGeocodingRequest(GoogleMapsRequestConfiguration api)
            : base(api, "geocode/json", "geocoding", false)
        {
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