namespace Juniper.World.GIS.Google.Geocoding;

/// <summary>
/// Note: This list is not exhaustive, and is subject to change. <see
/// cref="https://developers.google.com/maps/documentation/javascript/geocoding#GeocodingAddressTypes"/>
/// </summary>
[Flags]
public enum AddressComponentTypes : long
{
    None,

    Option1 = 1L,
    Option2 = 1L << 1,
    Option3 = 1L << 2,
    Option4 = 1L << 3,
    Option5 = 1L << 4,

    ///<summary>
    /// indicates a precise street address.
    /// </summary>
    street_address = 1L << 8,

    ///<summary>
    /// indicates a named route (such as "US 101").
    /// </summary>
    route = 1L << 9,

    ///<summary>
    /// indicates a major intersection, usually of two major roads.
    /// </summary>
    intersection = 1L << 10,

    ///<summary>
    /// indicates a political entity. Usually, this type indicates a polygon
    /// of some civil administration.
    /// </summary>
    political = 1L << 11,

    ///<summary>
    /// indicates the national political entity, and is typically the
    /// highest order type returned by the Geocoder.
    /// </summary>
    country = 1L << 12,

    administrative_area = 1L << 13,

    ///<summary>
    /// indicates a first-order civil entity below the country level. Within
    /// the United States, these administrative levels are states. Not all
    /// nations exhibit these administrative levels. In most cases,
    /// administrative_area_level_1 short names will closely match ISO
    /// 3166-2 subdivisions and other widely circulated lists; however this
    /// is not guaranteed as our geocoding results are based on a variety of
    /// signals and location data.
    /// </summary>
    administrative_area_level_1 = administrative_area | Option1,

    /// <summary>
    /// An alias for <see cref="administrative_area_level_1"/>.
    /// </summary>
    state = administrative_area_level_1,

    ///<summary>
    /// indicates a second-order civil entity below the country level.
    /// Within the United States, these administrative levels are counties.
    /// Not all nations exhibit these administrative levels.
    /// </summary>
    administrative_area_level_2 = administrative_area | Option2,

    /// <summary>
    /// An alias for <see cref="administrative_area_level_2"/>
    /// </summary>
    county = administrative_area_level_2,

    ///<summary>
    /// indicates a third-order civil entity below the country level. This
    /// type indicates a minor civil division. Not all nations exhibit these
    /// administrative levels.
    /// </summary>
    administrative_area_level_3 = administrative_area | Option3,

    /// <summary>
    /// An alias for <see cref="administrative_area_level_3"/>.
    /// </summary>
    township = administrative_area_level_3,

    ///<summary>
    /// indicates a fourth-order civil entity below the country level. This
    /// type indicates a minor civil division. Not all nations exhibit these
    /// administrative levels.
    /// </summary>
    administrative_area_level_4 = administrative_area | Option4,

    ///<summary>
    /// indicates a fifth-order civil entity below the country level. This
    /// type indicates a minor civil division. Not all nations exhibit these
    /// administrative levels.
    /// </summary>
    administrative_area_level_5 = administrative_area | Option5,

    ///<summary>
    /// indicates a commonly-used alternative name for the entity.
    /// </summary>
    colloquial_area = 1L << 14,

    ///<summary>
    /// indicates an incorporated city or town political entity.
    /// </summary>
    locality = 1L << 15,

    /// <summary>
    /// An alias for <see cref="locality"/>.
    /// </summary>
    city = locality,

    ///<summary>
    /// indicates a first-order civil entity below a locality. For some
    /// locations may receive one of the additional types:
    /// sublocality_level_1 to sublocality_level_5. Each sub-locality level
    /// is a civil entity. Larger numbers indicate a smaller geographic
    /// area.
    /// </summary>
    sublocality = 1L << 16,

    sublocality_level_1 = sublocality | Option1,
    sublocality_level_2 = sublocality | Option2,
    sublocality_level_3 = sublocality | Option3,
    sublocality_level_4 = sublocality | Option4,
    sublocality_level_5 = sublocality | Option5,

    ///<summary>
    /// indicates a named neighborhood
    /// </summary>
    neighborhood = 1L << 17,

    ///<summary>
    /// indicates a named location, usually a building or collection of
    /// buildings with a common name
    /// </summary>
    premise = 1L << 18,

    ///<summary>
    /// indicates a first-order entity below a named location, usually a
    /// singular building within a collection of buildings with a common
    /// name
    /// </summary>
    subpremise = 1L << 19,

    ///<summary>
    /// indicates a postal code as used to address postal mail within the
    /// country.
    /// </summary>
    postal_code = 1L << 20,

    postal_code_suffix = 1L << 21,

    ///<summary>
    /// indicates a prominent natural feature.
    /// </summary>
    natural_feature = 1L << 22,

    ///<summary>
    /// indicates a named park.
    /// </summary>
    park = 1L << 27,

    ///<summary>
    /// indicates a named point of interest. Typically, these "POI"s are
    /// prominent local entities that don't easily fit in another category,
    /// such as "Empire State Building" or "Eiffel Tower".
    /// </summary>
    point_of_interest = 1L << 28,

    ///<summary>
    /// indicates the floor of a building address.
    /// </summary>
    floor = 1L << 29,

    ///<summary>
    /// typically indicates a place that has not yet been categorized.
    /// </summary>
    establishment = 1L << 30,

    ///<summary>
    /// indicates a parking lot or parking structure.
    /// </summary>
    parking = 1L << 31,

    ///<summary>
    /// indicates a specific postal box.
    /// </summary>
    post_box = 1L << 32,

    ///<summary>
    /// indicates a grouping of geographic areas, such as locality and
    /// sub-locality, used for mailing addresses in some countries.
    /// </summary>
    postal_town = 1L << 33,

    ///<summary>
    /// indicates the room of a building address.
    /// </summary>
    room = 1L << 34,

    ///<summary>
    /// indicates the precise street number.
    /// </summary>
    street_number = 1L << 35,

    Port = 1L << 36,

    ///<summary>
    /// indicates an airport.
    /// </summary>
    airport = Port | Option1,

    ///<summary>
    /// indicates the location of a bus stop.
    /// </summary>
    bus_station = Port | Option2,

    ///<summary>
    /// indicates the location of a train stop.
    /// </summary>
    train_station = Port | Option3,

    ///<summary>
    /// indicates the location of a public transit stop.
    /// </summary>
    transit_station = Port | Option4
}