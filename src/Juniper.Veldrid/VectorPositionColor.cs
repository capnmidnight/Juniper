using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct VertexPositionColor : IEquatable<VertexPositionColor>
    {
        public const uint SizeInBytes = sizeof(float) * 7;

        public static readonly VertexLayoutDescription Layout =
            new VertexLayoutDescription(
                new VertexElementDescription(
                    nameof(Position),
                    VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float3),
                new VertexElementDescription(
                    nameof(Color),
                    VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float4));

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
