using System.IO;

namespace Juniper.World.GIS.Google.Geocoding
{
    public abstract class AbstractGeocodingRequest : AbstractGoogleMapsRequest<MediaType.Application>
    {
        private string language;

        protected AbstractGeocodingRequest(string apiKey)
            : base("geocode/json", apiKey, null, MediaType.Application.Json)
        {
        }

        public override string CacheID
        {
            get
            {
                return Path.Combine("geocoding", base.CacheID);
            }
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