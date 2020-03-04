using System;
using System.Numerics;

using Veldrid;

namespace Juniper
{
    public struct VertexPositionColor : IEquatable<VertexPositionColor>
    {
        public const uint SizeInBytes = sizeof(float) * 7;
        public Vector3 Position { get; }
        public RgbaFloat Color { get; }

        public VertexPositionColor(Vector3 position, RgbaFloat color)
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
            return HashCode.Combine(Position, Color);
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
