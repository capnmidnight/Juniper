using System.Collections.Generic;

namespace Juniper.World.GIS.Google.Geocoding
{
    public class GeocodingRequest : AbstractGeocodingRequest
    {
        private string place_id;
        private string address;

        private readonly Dictionary<AddressComponentTypes, string> components = new Dictionary<AddressComponentTypes, string>();

        private GeometryViewport bounds;
        private string region;

        public GeocodingRequest(string apiKey)
            : base(apiKey)
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

        private string SetComponent(AddressComponentTypes key, string value)
        {
            components[key] = value;
            RefreshComponents();
            return value;
        }

        public string PostalCodeFilter
        {
            get { return components.Get(AddressComponentTypes.postal_code, default); }
            set { SetComponent(AddressComponentTypes.postal_code, value); }
        }

        public string CountryFilter
        {
            get { return components.Get(AddressComponentTypes.country, default); }
            set { SetComponent(AddressComponentTypes.country, value); }
        }

        public string RouteHint
        {
            get { return components.Get(AddressComponentTypes.route, default); }
            set { SetComponent(AddressComponentTypes.route, value); }
        }

        public string LocalityHint
        {
            get { return components.Get(AddressComponentTypes.locality, default); }
            set { SetComponent(AddressComponentTypes.locality, value); }
        }

        public string AdministrativeAreaHint
        {
            get { return components.Get(AddressComponentTypes.administrative_area, default); }
            set { SetComponent(AddressComponentTypes.administrative_area, value); }
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
                SetQuery(nameof(bounds), $"{bounds.SouthWest}|{bounds.NorthEast}");
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