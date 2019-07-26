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

        private GeometryViewport bounds;
        private string region;

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

        public string PostalCodeFilter
        {
            get { return components.Get(AddressComponentType.postal_code, default); }
            set { SetComponent(AddressComponentType.postal_code, value); }
        }

        public string CountryFilter
        {
            get { return components.Get(AddressComponentType.country, default); }
            set { SetComponent(AddressComponentType.country, value); }
        }

        public string RouteHint
        {
            get { return components.Get(AddressComponentType.route, default); }
            set { SetComponent(AddressComponentType.route, value); }
        }

        public string LocalityHint
        {
            get { return components.Get(AddressComponentType.locality, default); }
            set { SetComponent(AddressComponentType.locality, value); }
        }

        public string AdministrativeAreaHint
        {
            get { return components.Get(AddressComponentType.administrative_area, default); }
            set { SetComponent(AddressComponentType.administrative_area, value); }
        }

        public void SetBounds(LatLngPoint southWest, LatLngPoint northEast)
        {
            Bounds = new GeometryViewport(southWest, northEast);
        }

        public GeometryViewport Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                SetQuery(nameof(bounds), $"{bounds.southwest.ToCSV()}|{bounds.northeast.ToCSV()}");
            }
        }

        public string Region
        {
            get { return region; }
            set { region = SetQuery(nameof(region), region); }
        }
    }
}