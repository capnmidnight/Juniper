using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public class GeocodingResult : ISerializable
    {
        public bool partial_match { get; }

        private static readonly string ADDRESS_COMPONENTS_FIELD = nameof(Address_Components).ToLowerInvariant();
        private static readonly string FORMATTED_ADDRESS_FIELD = nameof(Formatted_Address).ToLowerInvariant();
        private static readonly string PLACE_ID_FIELD = nameof(Place_ID).ToLowerInvariant();
        private static readonly string TYPES_FIELD = nameof(Types).ToLowerInvariant();
        private static readonly string GEOMETRY_FIELD = nameof(Geometry).ToLowerInvariant();
        private static readonly string PARTIAL_MATCH_FIELD = nameof(partial_match).ToLowerInvariant();

        private readonly Dictionary<int, AddressComponent> addressComponentLookup;

        public AddressComponent[] Address_Components { get; }

        public string Formatted_Address { get; }

        public string Place_ID { get; }

        public string[] TypeStrings { get; }

        public HashSet<AddressComponentTypes> Types { get; }

        public GeometryResult Geometry { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected GeocodingResult(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Address_Components = info.GetValue<AddressComponent[]>(ADDRESS_COMPONENTS_FIELD);
            addressComponentLookup = Address_Components.ToDictionary(elem => elem.Key);
            Formatted_Address = info.GetString(FORMATTED_ADDRESS_FIELD);
            Place_ID = info.GetString(PLACE_ID_FIELD);
            TypeStrings = info.GetValue<string[]>(TYPES_FIELD);
            Types = new HashSet<AddressComponentTypes>(from typeStr in TypeStrings
                                                       select Enum.TryParse<AddressComponentTypes>(typeStr, out var parsedType)
                                                           ? parsedType
                                                           : AddressComponentTypes.None);
            Geometry = info.GetValue<GeometryResult>(GEOMETRY_FIELD);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (partial_match)
            {
                info.AddValue(PARTIAL_MATCH_FIELD, partial_match);
            }

            info.AddValue(ADDRESS_COMPONENTS_FIELD, Address_Components);
            info.AddValue(FORMATTED_ADDRESS_FIELD, Formatted_Address);
            info.AddValue(PLACE_ID_FIELD, Place_ID);
            info.AddValue(TYPES_FIELD, TypeStrings);
            info.AddValue(GEOMETRY_FIELD, Geometry);
        }

        public AddressComponent GetAddressComponent(params AddressComponentTypes[] types)
        {
            var key = AddressComponent.HashAddressComponents(types);
            var results = addressComponentLookup.Get(key);
            if (results is null)
            {
                var subTypes = types.Append(AddressComponentTypes.political);
                var subKey = AddressComponent.HashAddressComponents(subTypes);
                results = addressComponentLookup.Get(subKey);
            }

            return results;
        }
    }
}