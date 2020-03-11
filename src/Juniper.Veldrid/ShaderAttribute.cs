using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Juniper.VeldridIntegration
{
    public struct ShaderAttribute : IEquatable<ShaderAttribute>
    {
        private readonly ShaderLayout layout;
        public int Location => layout.Location;
        public int Component => layout.Component;
        public int Index => layout.Index;

        public string Name { get; }
        public ShaderAttributeDirection Direction { get; }
        public string DataType { get; }
        public uint Size { get; }

        public ShaderAttribute(int index, string name, ShaderAttributeDirection direction, string dataType, ShaderLayoutQualifier[] qualifiers)
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

            layout = new ShaderLayout(index, qualifiers);
        }

        public override string ToString()
        {
            var sb = new StringBuilder(layout.ToString());
            sb.AppendFormat(CultureInfo.InvariantCulture, "{0}{1} {2} {3}", layout, Direction.ToString().ToLowerInvariant(), DataType, Name);
            return sb.ToString();
        }

        private bool SubMatch(ShaderAttribute other)
        {
            return Name == other.Name
                && DataType == other.DataType
                && layout == other.layout;
        }

        public bool PipesTo(ShaderAttribute other)
        {
            return Direction == ShaderAttributeDirection.Out
                && other.Direction == ShaderAttributeDirection.In
                && SubMatch(other);
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderAttribute attribute && Equals(attribute);
        }

        public bool Equals(ShaderAttribute other)
        {
            return Name == other.Name
                && Direction == other.Direction
                && DataType == other.DataType
                && Size == other.Size
                && layout.Equals(other.layout);
        }

        public override int GetHashCode()
        {
            var hashCode = 1631821752;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DataType);
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + layout.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ShaderAttribute left, ShaderAttribute right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderAttribute left, ShaderAttribute right)
        {
            return !(left == right);
        }
    }
}
