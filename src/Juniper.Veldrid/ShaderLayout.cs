using System;
using System.Text;

namespace Juniper.VeldridIntegration
{
    public struct ShaderLayout : IEquatable<ShaderLayout>
    {
        private readonly ShaderLayoutQualifier[] qualifiers;

        public int Binding { get; }
        public int Set { get; }
        public int Location { get; }
        public int Component { get; }
        public int Index { get; }

        public ShaderLayout(int index, ShaderLayoutQualifier[] qualifiers)
        {
            this.qualifiers = qualifiers ?? throw new ArgumentNullException(nameof(qualifiers));
            Binding = -1;
            Set = -1;
            Location = index;
            Component = 0;
            Index = -1;

            foreach (var qualifier in qualifiers)
            {
                var value = (int)qualifier.Value;
                switch (qualifier.Type)
                {
                    case ShaderLayoutQualifierType.Binding:
                    Binding = value;
                    break;

                    case ShaderLayoutQualifierType.Set:
                    Set = value;
                    break;

                    case ShaderLayoutQualifierType.Location:
                    Location = value;
                    break;

                    case ShaderLayoutQualifierType.Component:
                    Component = value;
                    break;

                    case ShaderLayoutQualifierType.Index:
                    Index = value;
                    break;
                }
            }
        }

        public override string ToString()
        {
            if (qualifiers.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder("layout(");
            foreach (var qualifier in qualifiers)
            {
                sb.Append(qualifier.ToString());
            }
            sb.Append(") ");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is ShaderLayout layout
                && Equals(layout);
        }

        public bool Equals(ShaderLayout other)
        {
            return Binding == other.Binding
                && Set == other.Set
                && Location == other.Location
                && Component == other.Component
                && Index == other.Index;
        }

        public override int GetHashCode()
        {
            var hashCode = 2131007298;
            hashCode = hashCode * -1521134295 + Binding.GetHashCode();
            hashCode = hashCode * -1521134295 + Set.GetHashCode();
            hashCode = hashCode * -1521134295 + Location.GetHashCode();
            hashCode = hashCode * -1521134295 + Component.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ShaderLayout left, ShaderLayout right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderLayout left, ShaderLayout right)
        {
            return !(left == right);
        }
    }
}
