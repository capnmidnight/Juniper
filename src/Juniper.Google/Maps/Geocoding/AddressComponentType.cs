using System;

namespace Juniper.Google.Maps.Geocoding
{
    /// <summary>
    /// Note: This list is not exhaustive, and is subject to change. <see
    /// cref="https://developers.google.com/maps/documentation/javascript/geocoding#GeocodingAddressTypes"/>
    /// </summary>
    [Flags]
    public enum AddressComponentType : long
    {
        Unknown = 0,

        Option1 = 0x01,
        Option2 = 0x02,
        Option3 = 0x04,
        Option4 = 0x08,
        Option5 = 0x10,

        ///<summary>
        /// indicates a precise street address.
        /// </summary>
        street_address = 0x00000000100,

        ///<summary>
        /// indicates a named route (such as "US 101").
        /// </summary>
        route = 0x00000000200,

        ///<summary>
        /// indicates a major intersection, usually of two major roads.
        /// </summary>
        intersection = 0x00000000400,

        ///<summary>
        /// indicates a political entity. Usually, this type indicates a polygon
        /// of some civil administration.
        /// </summary>
        political = 0x00000000800,

        ///<summary>
        /// indicates the national political entity, and is typically the
        /// highest order type returned by the Geocoder.
        /// </summary>
        country = 0x00000001000,

        administrative_area = 0x00000002000,

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
        colloquial_area = 0x00000004000,

        ///<summary>
        /// indicates an incorporated city or town political entity.
        /// </summary>
        locality = 0x00000008000,

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
        sublocality = 0x00000010000,

        sublocality_level_1 = sublocality | Option1,
        sublocality_level_2 = sublocality | Option2,
        sublocality_level_3 = sublocality | Option3,
        sublocality_level_4 = sublocality | Option4,
        sublocality_level_5 = sublocality | Option5,

        ///<summary>
        /// indicates a named neighborhood
        /// </summary>
        neighborhood = 0x00000020000,

        ///<summary>
        /// indicates a named location, usually a building or collection of
        /// buildings with a common name
        /// </summary>
        premise = 0x000000040000,

        ///<summary>
        /// indicates a first-order entity below a named location, usually a
        /// singular building within a collection of buildings with a common
        /// name
        /// </summary>
        subpremise = 0x00000080000,

        ///<summary>
        /// indicates a postal code as used to address postal mail within the
        /// country.
        /// </summary>
        postal_code = 0x00000100000,

        postal_code_suffix = 0x00000200000,

        ///<summary>
        /// indicates a prominent natural feature.
        /// </summary>
        natural_feature = 0x00000400000,

        ///<summary>
        /// indicates a named park.
        /// </summary>
        park = 0x000008000000,

        ///<summary>
        /// indicates a named point of interest. Typically, these "POI"s are
        /// prominent local entities that don't easily fit in another category,
        /// such as "Empire State Building" or "Eiffel Tower".
        /// </summary>
        point_of_interest = 0x00010000000,

        ///<summary>
        /// indicates the floor of a building address.
        /// </summary>
        floor = 0x00020000000,

        ///<summary>
        /// typically indicates a place that has not yet been categorized.
        /// </summary>
        establishment = 0x00040000000,

        ///<summary>
        /// indicates a parking lot or parking structure.
        /// </summary>
        parking = 0x00080000000,

        ///<summary>
        /// indicates a specific postal box.
        /// </summary>
        post_box = 0x00100000000,

        ///<summary>
        /// indicates a grouping of geographic areas, such as locality and
        /// sub-locality, used for mailing addresses in some countries.
        /// </summary>
        postal_town = 0x00200000000,

        ///<summary>
        /// indicates the room of a building address.
        /// </summary>
        room = 0x00400000000,

        ///<summary>
        /// indicates the precise street number.
        /// </summary>
        street_number = 0x00800000000,

        Port = 0x01000000000,

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
}