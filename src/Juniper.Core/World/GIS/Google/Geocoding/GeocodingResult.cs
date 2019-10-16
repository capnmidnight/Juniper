using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public class GeocodingResult : ISerializable
    {
        public readonly bool partial_match;
        public readonly AddressComponent[] address_components;
        private readonly Dictionary<int, AddressComponent> addressComponentLookup;
        public readonly string formatted_address;
        public readonly string place_id;
        public readonly string[] typeStrings;
        public readonly HashSet<AddressComponentType> types;
        public readonly GeometryResult geometry;

        protected GeocodingResult(SerializationInfo info, StreamingContext context)
        {
            address_components = info.GetValue<AddressComponent[]>(nameof(address_components));
            addressComponentLookup = address_components.ToDictionary(elem => elem.Key);
            formatted_address = info.GetString(nameof(formatted_address));
            place_id = info.GetString(nameof(place_id));
            typeStrings = info.GetValue<string[]>(nameof(types));
            types = new HashSet<AddressComponentType>(from typeStr in typeStrings
                                                      select Enum.TryParse<AddressComponentType>(typeStr, out var parsedType)
                                                          ? parsedType
                                                          : AddressComponentType.Unknown);
            geometry = info.GetValue<GeometryResult>(nameof(geometry));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (partial_match)
            {
                info.AddValue(nameof(partial_match), partial_match);
            }
            info.AddValue(nameof(address_components), address_components);
            info.AddValue(nameof(formatted_address), formatted_address);
            info.AddValue(nameof(place_id), place_id);
            info.AddValue(nameof(types), typeStrings);
            info.AddValue(nameof(geometry), geometry);
        }

        public AddressComponent GetAddressComponent(params AddressComponentType[] types)
        {
            var key = AddressComponent.HashAddressComponents(types);
            var results = addressComponentLookup.Get(key);
            if (results == null)
            {
                var subTypes = types.Append(AddressComponentType.political);
                var subKey = AddressComponent.HashAddressComponents(subTypes);
                results = addressComponentLookup.Get(subKey);
            }

            return results;
        }
    }
}