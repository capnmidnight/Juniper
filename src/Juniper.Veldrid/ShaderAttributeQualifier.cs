using System;
using System.Globalization;

namespace Juniper.VeldridIntegration
{
    public struct ShaderAttributeQualifier : IEquatable<ShaderAttributeQualifier>
    {
        public ShaderAttributeQualifierType Type { get; }
        public uint Value { get; }

        public ShaderAttributeQualifier(ShaderAttributeQualifierType type, uint value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Type.ToString().ToLowerInvariant()} = {Value.ToString(CultureInfo.InvariantCulture)}";
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderAttributeQualifier qualifier && Equals(qualifier);
        }

        public bool Equals(ShaderAttributeQualifier other)
        {
            return Type == other.Type &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = 1265339359;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ShaderAttributeQualifier left, ShaderAttributeQualifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderAttributeQualifier left, ShaderAttributeQualifier right)
        {
            return !(left == right);
        }
    }
}
