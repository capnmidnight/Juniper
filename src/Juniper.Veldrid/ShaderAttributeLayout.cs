using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Juniper.VeldridIntegration
{
    public struct ShaderAttributeLayout : IEquatable<ShaderAttributeLayout>
    {
        public string Name { get; }
        public ShaderAttributeDirection Direction { get; }
        public string DataType { get; }
        public uint Size { get; }

        public ShaderAttributeQualifier[] Qualifiers { get; }
        public int Location { get; }
        public int Offset { get; }

        public ShaderAttributeLayout(int index, string name, ShaderAttributeDirection direction, string dataType, ShaderAttributeQualifier[] qualifiers)
        {
            Name = name;
            Direction = direction;
            DataType = dataType;
            Size = DataType switch
            {
                "int" => 4,
                "ivec2" => 8,
                "ivec3" => 16,
                "ivec4" => 32,
                "uint" => 4,
                "uvec2" => 8,
                "uvec3" => 16,
                "uvec4" => 32,
                "float" => 4,
                "vec2" => 8,
                "vec3" => 16,
                "vec4" => 32,
                "double" => 8,
                "dvec2" => 32,
                "dvec3" => 64,
                "dvec4" => 128,
                _ => throw new FormatException($"Unrecognized data type {DataType}")
            };

            Qualifiers = qualifiers ?? throw new ArgumentNullException(nameof(qualifiers));
            Location = index;
            Offset = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Qualifiers.Length > 0)
            {
                sb.Append("layout(");
                foreach (var qualifier in Qualifiers)
                {
                    sb.Append(qualifier.ToString());
                }
                sb.Append(") ");
            }
            _ = sb.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2}", Direction.ToString().ToLowerInvariant(), DataType, Name);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderAttributeLayout layout && Equals(layout);
        }

        public bool Equals(ShaderAttributeLayout other)
        {
            return Direction == other.Direction
                && SubMatch(other);
        }

        private bool SubMatch(ShaderAttributeLayout other)
        {
            return Name == other.Name
                && DataType == other.DataType
                && Qualifiers.Length == other.Qualifiers.Length
                && Qualifiers.All(q => other.Qualifiers.Contains(q));
        }

        public bool PipesTo(ShaderAttributeLayout other)
        {
            return Direction == ShaderAttributeDirection.Out
                && other.Direction == ShaderAttributeDirection.In
                && SubMatch(other);
        }

        public override int GetHashCode()
        {
            var hashCode = 892080441;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DataType);
            hashCode = hashCode * -1521134295 + EqualityComparer<ShaderAttributeQualifier[]>.Default.GetHashCode(Qualifiers);
            return hashCode;
        }

        public static bool operator ==(ShaderAttributeLayout left, ShaderAttributeLayout right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderAttributeLayout left, ShaderAttributeLayout right)
        {
            return !(left == right);
        }
    }
}
