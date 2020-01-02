using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.World.GIS.Google.Geocoding
{
    [Serializable]
    public sealed class AddressComponent : ISerializable, IEquatable<AddressComponent>
    {
        public static int HashAddressComponents(IEnumerable<AddressComponentTypes> types)
        {
            var key = 0;
            foreach (var type in types)
            {
                key ^= type.GetHashCode();
            }

            return key;
        }

        public readonly string long_name;
        public readonly string short_name;
        public readonly string[] typeStrings;
        public readonly HashSet<AddressComponentTypes> types;

        internal int Key { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private AddressComponent(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            long_name = info.GetString(nameof(long_name));
            short_name = info.GetString(nameof(short_name));
            typeStrings = info.GetValue<string[]>(nameof(types));
            types = new HashSet<AddressComponentTypes>(from typeStr in typeStrings
                                                       select Enum.TryParse<AddressComponentTypes>(typeStr, out var parsedType)
                                                           ? parsedType
                                                           : AddressComponentTypes.None);

            Key = HashAddressComponents(types);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(long_name), long_name);
            info.AddValue(nameof(short_name), short_name);
            info.AddValue(nameof(types), typeStrings);
        }

        public override int GetHashCode()
        {
            return long_name.GetHashCode() ^ Key;
        }

        public override bool Equals(object obj)
        {
            return obj is AddressComponent addr && Equals(addr);
        }

        public bool Equals(AddressComponent other)
        {
            return other is object
                && Key == other.Key
                && long_name == other.long_name;
        }

        public static bool operator ==(AddressComponent left, AddressComponent right)
        {
            return ReferenceEquals(left, right)
                || (left is object && left.Equals(right));
        }

        public static bool operator !=(AddressComponent left, AddressComponent right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            var t = typeStrings.ToString("|");

            return $"{t}:{long_name}";
        }
    }
}