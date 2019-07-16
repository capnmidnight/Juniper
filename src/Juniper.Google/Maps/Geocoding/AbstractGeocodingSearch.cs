using System;

namespace Juniper.Google.Maps.Geocoding
{
    public abstract class AbstractGeocodingSearch<T> : AbstractMapsSearch<T>
    {
        protected AbstractGeocodingSearch(USAddress address)
            : base("geocode/json", "geocoding", "json")
        {
            uriBuilder.AddQuery(nameof(address), address);
        }
    }
}