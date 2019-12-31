using System.Collections.Generic;

namespace Juniper.World.GIS.Google.Geocoding
{
    public class ReverseGeocodingRequest : AbstractGeocodingRequest
    {
        private static readonly HashSet<AddressComponentTypes> AcceptableResultTypes = new HashSet<AddressComponentTypes>(new[]{
            AddressComponentTypes.street_address,
            AddressComponentTypes.route,
            AddressComponentTypes.intersection,
            AddressComponentTypes.political,
            AddressComponentTypes.country,
            AddressComponentTypes.administrative_area_level_1,
            AddressComponentTypes.administrative_area_level_2,
            AddressComponentTypes.administrative_area_level_3,
            AddressComponentTypes.administrative_area_level_4,
            AddressComponentTypes.administrative_area_level_5,
            AddressComponentTypes.colloquial_area,
            AddressComponentTypes.locality,
            AddressComponentTypes.sublocality,
            AddressComponentTypes.neighborhood,
            AddressComponentTypes.premise,
            AddressComponentTypes.subpremise,
            AddressComponentTypes.postal_code,
            AddressComponentTypes.natural_feature,
            AddressComponentTypes.airport,
            AddressComponentTypes.park,
            AddressComponentTypes.point_of_interest
        });

        private readonly HashSet<AddressComponentTypes> result_type = new HashSet<AddressComponentTypes>();
        private readonly HashSet<GeometryLocationType> location_type = new HashSet<GeometryLocationType>();
        private LatLngPoint latlng;

        public ReverseGeocodingRequest(string apiKey)
            : base(apiKey)
        { }

        public LatLngPoint Location
        {
            get { return latlng; }
            set
            {
                latlng = value;
                SetQuery(nameof(latlng), latlng);
            }
        }

        public void AddResultType(AddressComponentTypes value)
        {
            if (AcceptableResultTypes.Contains(value))
            {
                result_type.Add(value);
                SetQuery(nameof(result_type), result_type.ToString("|"));
            }
        }

        public void AddLocationType(GeometryLocationType value)
        {
            if (value != GeometryLocationType.Unknown)
            {
                location_type.Add(value);
                SetQuery(nameof(location_type), location_type.ToString("|"));
            }
        }
    }
}