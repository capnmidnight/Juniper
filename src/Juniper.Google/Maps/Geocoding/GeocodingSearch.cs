using System;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class GeocodingSearch : AbstractMapsSearch
    {
        private GeocodingSearch(USAddress address)
            : base("geocode/json", "json")
        {
            uriBuilder.AddQuery(nameof(address), address);
        }
    }
}