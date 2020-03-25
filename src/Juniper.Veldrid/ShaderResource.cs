using System;
using System.Collections.Generic;
using System.Linq;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct ShaderResource : IEquatable<ShaderResource>
    {
        private readonly ShaderLayout layout;

        public int Binding => layout.Binding;

        public int Set => layout.Set;

        public string Name { get; }

        public ResourceKind Kind { get; }

        public ShaderStages Stage { get; }

        private readonly (ShaderDataType type, string name, uint size)[] identifiers;

        public ShaderResource(string name, ShaderLayoutQualifier[] qualifiers, ResourceKind kind, ShaderStages stage, params (ShaderDataType type, string name, uint size)[] identifiers)
        {
            layout = new ShaderLayout(0, qualifiers);
            Name = name;
            Kind = kind;
            Stage = stage;
            this.identifiers = identifiers;
        }

        public uint Size =>
            (uint)identifiers.Sum(i => i.type.Size() * i.size);

        public override bool Equals(object obj)
        {
            return obj is ShaderResource resource && Equals(resource);
        }

        public bool Equals(ShaderResource other)
        {
            return layout.Equals(other.layout) &&
                   Binding == other.Binding &&
                   Set == other.Set &&
                   Name == other.Name &&
                   Kind == other.Kind &&
                   Stage == other.Stage;
        }

        public override int GetHashCode()
        {
            var hashCode = -1931169198;
            hashCode = hashCode * -1521134295 + layout.GetHashCode();
            hashCode = hashCode * -1521134295 + Binding.GetHashCode();
            hashCode = hashCode * -1521134295 + Set.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Kind.GetHashCode();
            hashCode = hashCode * -1521134295 + Stage.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ShaderResource left, ShaderResource right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShaderResource left, ShaderResource right)
        {
            return !(left == right);
        }
    }
}
