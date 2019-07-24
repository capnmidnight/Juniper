using System.Collections.Generic;

using Juniper.World.GIS;

namespace Juniper.Google.Maps.Geocoding
{
    public class GeocodingRequest : AbstractGeocodingRequest
    {
        public static GeocodingRequest Create(LocationTypes locationType, object value)
        {
            switch (locationType)
            {
                case LocationTypes.None: return new GeocodingRequest();
                case LocationTypes.PlaceName: return new GeocodingRequest((PlaceName)value);
                default: return default;
            }
        }

        private readonly Dictionary<AddressComponentType, string> components = new Dictionary<AddressComponentType, string>();

        public GeocodingRequest()
        {
        }

        private GeocodingRequest(string address)
            : base(nameof(address), address) { }

        public GeocodingRequest(PlaceID place_id)
            : base(nameof(place_id), (string)place_id) { }

        public GeocodingRequest(PlaceName address)
            : this((string)address) { }

        public GeocodingRequest(IDictionary<AddressComponentType, string> components)
        {
            foreach (var kv in components)
            {
                this.components[kv.Key] = kv.Value;
            }
            RefreshComponents();
        }

        private void RefreshComponents()
        {
            SetQuery(nameof(components), components.ToString(":", "|"));
        }

        private string SetComponent(AddressComponentType key, string value)
        {
            components[key] = value;
            RefreshComponents();
            return value;
        }

        public void SetPostalCodeFilter(string value)
        {
            SetComponent(AddressComponentType.postal_code, value);
        }

        public void SetCountryFilter(string value)
        {
            SetComponent(AddressComponentType.country, value);
        }

        public void SetRouteHint(string value)
        {
            SetComponent(AddressComponentType.route, value);
        }

        public void SetLocalityHint(string value)
        {
            SetComponent(AddressComponentType.locality, value);
        }

        public void SetAdministrativeAreaHint(string value)
        {
            SetComponent(AddressComponentType.administrative_area, value);
        }

        public void SetBounds(LatLngPoint southWest, LatLngPoint northEast)
        {
            SetBounds(new GeometryViewport(southWest, northEast));
        }

        public void SetBounds(GeometryViewport bounds)
        {
            SetQuery(nameof(bounds), $"{bounds.southwest.ToCSV()}|{bounds.northeast.ToCSV()}");
        }

        public void SetRegion(string region)
        {
            SetQuery(nameof(region), region);
        }
    }
}