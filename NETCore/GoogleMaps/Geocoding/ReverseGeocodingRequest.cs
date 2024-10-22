using System.Globalization;

namespace Juniper.World.GIS.Google.Geocoding;

public class ReverseGeocodingRequest : AbstractGeocodingRequest
{
    private static readonly HashSet<AddressComponentTypes> AcceptableResultTypes = new(new[]{
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

    private readonly HashSet<AddressComponentTypes> result_type = new();
    private readonly HashSet<GeometryLocationType> location_type = new();
    private LatLngPoint? latlng;

    public ReverseGeocodingRequest(HttpClient http, string apiKey)
        : base(http, apiKey)
    { }

    public LatLngPoint? Location
    {
        get => latlng;
        set
        {
            latlng = value;
            SetQuery(nameof(latlng), latlng?.ToString(CultureInfo.InvariantCulture));
        }
    }

    public void AddResultType(AddressComponentTypes value)
    {
        if (AcceptableResultTypes.Contains(value))
        {
            result_type.Add(value);
            SetQuery(nameof(result_type), result_type?.ToString("|"));
        }
    }

    public void AddLocationType(GeometryLocationType value)
    {
        if (value != GeometryLocationType.Unknown)
        {
            location_type.Add(value);
            SetQuery(nameof(location_type), location_type?.ToString("|"));
        }
    }
}