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
            get => place_id;
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
            get => address;
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
            SetQuery(nameof(components), components?.ToString(":", "|"));
        }

        private string SetComponent(AddressComponentTypes key, string value)
        {
            components[key] = value;
            RefreshComponents();
            return value;
        }

        public string PostalCodeFilter
        {
            get => components.Get(AddressComponentTypes.postal_code, default);
            set => SetComponent(AddressComponentTypes.postal_code, value);
        }

        public string CountryFilter
        {
            get => components.Get(AddressComponentTypes.country, default);
            set => SetComponent(AddressComponentTypes.country, value);
        }

        public string RouteHint
        {
            get => components.Get(AddressComponentTypes.route, default);
            set => SetComponent(AddressComponentTypes.route, value);
        }

        public string LocalityHint
        {
            get => components.Get(AddressComponentTypes.locality, default);
            set => SetComponent(AddressComponentTypes.locality, value);
        }

        public string AdministrativeAreaHint
        {
            get => components.Get(AddressComponentTypes.administrative_area, default);
            set => SetComponent(AddressComponentTypes.administrative_area, value);
        }

        public void SetBounds(LatLngPoint southWest, LatLngPoint northEast)
        {
            Bounds = new GeometryViewport(southWest, northEast);
        }

        public GeometryViewport Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                SetQuery(
                    nameof(bounds),
                    bounds == null
                        ? null
                        : $"{bounds.SouthWest}|{bounds.NorthEast}");
            }
        }

        public string Region
        {
            get => region;
            set
            {
                region = value;
                SetQuery(nameof(region), region);
            }
        }
    }
}