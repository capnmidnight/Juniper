using System.IO;

namespace Juniper.World.GIS.Google.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractGoogleMapsRequest<MediaType.Application>
    {
        private string language;

        protected AbstractGeocodingRequest(string apiKey)
            : base("geocode/json", apiKey, null, Juniper.MediaType.Application.Json)
        {
        }

        public override string CacheID =>
            Path.Combine("geocoding", base.CacheID);

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