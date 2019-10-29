using System.Collections.Generic;

namespace Juniper.World.GIS.Google.Geocoding
{
    public class ReverseGeocodingRequest : AbstractGeocodingRequest
    {
        private static readonly HashSet<AddressComponentType> AcceptableResultTypes = new HashSet<AddressComponentType>(new[]{
            AddressComponentType.street_address,
            AddressComponentType.route,
            AddressComponentType.intersection,
            AddressComponentType.political,
            AddressComponentType.country,
            AddressComponentType.administrative_area_level_1,
            AddressComponentType.administrative_area_level_2,
            AddressComponentType.administrative_area_level_3,
            AddressComponentType.administrative_area_level_4,
            AddressComponentType.administrative_area_level_5,
            AddressComponentType.colloquial_area,
            AddressComponentType.locality,
            AddressComponentType.sublocality,
            AddressComponentType.neighborhood,
            AddressComponentType.premise,
            AddressComponentType.subpremise,
            AddressComponentType.postal_code,
            AddressComponentType.natural_feature,
            AddressComponentType.airport,
            AddressComponentType.park,
            AddressComponentType.point_of_interest
        });

        private readonly HashSet<AddressComponentType> result_type = new HashSet<AddressComponentType>();
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

        public void AddResultType(AddressComponentType value)
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