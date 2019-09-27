using System.Collections.Generic;
using System.IO;
using Juniper.World.GIS;

namespace Juniper.Google.Maps.Geocoding
{
    public class GeocodingRequest : AbstractGeocodingRequest
    {
        private string place_id;
        private string address;

        private readonly Dictionary<AddressComponentType, string> components = new Dictionary<AddressComponentType, string>();

        private GeometryViewport bounds;
        private string region;

        public GeocodingRequest(string apiKey, DirectoryInfo cacheLocation, IDictionary<AddressComponentType, string> components)
            : base(apiKey, cacheLocation)
        {
            if (components != null)
            {
                foreach (var kv in components)
                {
                    this.components[kv.Key] = kv.Value;
                }
                RefreshComponents();
            }
        }

        public GeocodingRequest(string apiKey, DirectoryInfo cacheLocation)
            : this(apiKey, cacheLocation, null)
        { }

        public GeocodingRequest(string apiKey, IDictionary<AddressComponentType, string> components)
            : this(apiKey, null, components)
        { }

        public GeocodingRequest(string apiKey)
            : this(apiKey, null, null)
        { }

        public string Place
        {
            get { return place_id; }
            set
            {
                place_id = value;
                SetQuery(nameof(place_id), place_id);

                address = default;
                RemoveQuery(nameof(address));
            }
        }

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                SetQuery(nameof(address), address);

                place_id = default;
                RemoveQuery(nameof(place_id));
            }
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
                SetQuery(nameof(bounds), $"{bounds.southwest}|{bounds.northeast}");
            }
        }

        public string Region
        {
            get { return region; }
            set
            {
                region = value;
                SetQuery(nameof(region), region);
            }
        }
    }
}