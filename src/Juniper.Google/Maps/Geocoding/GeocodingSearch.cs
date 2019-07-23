using System.Collections.Generic;

using Juniper.World.GIS;

namespace Juniper.Google.Maps.Geocoding
{
    public class GeocodingSearch : AbstractGeocodingSearch
    {
        private readonly Dictionary<AddressComponentType, string> components = new Dictionary<AddressComponentType, string>();

        private GeocodingSearch()
            : base() { }

        public GeocodingSearch(string address)
            : base(nameof(address), address) { }

        public GeocodingSearch(USAddress address)
            : this((string)address) { }

        public GeocodingSearch(Dictionary<AddressComponentType, string> components)
            : this()
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

        private void SetComponent(AddressComponentType key, string value)
        {
            components[key] = value;
            RefreshComponents();
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