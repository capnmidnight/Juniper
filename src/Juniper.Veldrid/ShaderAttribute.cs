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
        public ShaderDataType DataType { get; }

        public ShaderAttribute(int index, string name, ShaderAttributeDirection direction, ShaderDataType dataType, ShaderLayoutQualifier[] qualifiers)
        {
            Name = name;
            Direction = direction;
            DataType = dataType;

            layout = new ShaderLayout(index, qualifiers);
        }

        public override string ToString()
        {
            var sb = new StringBuilder(layout.ToString());
            return sb
                .AppendFormat(CultureInfo.InvariantCulture, "{0}{1} {2} {3}", layout, Direction.ToString().ToLowerInvariant(), DataType, Name)
                .ToString();
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
            return obj is ShaderAttribute attribute
                && Equals(attribute);
        }

        public bool Equals(ShaderAttribute other)
        {
            return layout.Equals(other.layout)
                && Location == other.Location
                && Component == other.Component
                && Index == other.Index
                && Name == other.Name
                && Direction == other.Direction
                && DataType == other.DataType;
        }

        public override int GetHashCode()
        {
            var hashCode = -99050907;
            hashCode = hashCode * -1521134295 + layout.GetHashCode();
            hashCode = hashCode * -1521134295 + Location.GetHashCode();
            hashCode = hashCode * -1521134295 + Component.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            hashCode = hashCode * -1521134295 + DataType.GetHashCode();
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
