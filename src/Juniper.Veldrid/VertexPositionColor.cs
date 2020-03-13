using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct VertexPositionColor : IEquatable<VertexPositionColor>
    {
        [VertexElement(VertexElementSemantic.Position)]
        public Vector2 Position { get; }

        [VertexElement(VertexElementSemantic.Color)]
        public RgbaFloat Color { get; }

        public VertexPositionColor(Vector2 position, RgbaFloat color)
        {
            Position = position;
            Color = color;
        }

        public override bool Equals(object obj)
        {
            return obj is VertexPositionColor vert
                && Equals(vert);
        }

        public bool Equals(VertexPositionColor vert)
        {
            return vert.Position.Equals(Position)
                && vert.Color.Equals(Color);
        }

        public override int GetHashCode()
        {
            var hashCode = -866678350;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Color.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(VertexPositionColor left, VertexPositionColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionColor left, VertexPositionColor right)
        {
            return !(left == right);
        }
    }
}
