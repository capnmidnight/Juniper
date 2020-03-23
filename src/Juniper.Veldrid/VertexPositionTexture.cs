using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct VertexPositionTexture : IEquatable<VertexPositionTexture>
    {
        [VertexElement(VertexElementSemantic.Position)]
        public Vector3 Position { get; }

        [VertexElement(VertexElementSemantic.TextureCoordinate)]
        public Vector2 UV { get; }

        public VertexPositionTexture(Vector3 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        public override bool Equals(object obj)
        {
            return obj is VertexPositionTexture vert
                && Equals(vert);
        }

        public bool Equals(VertexPositionTexture vert)
        {
            return vert.Position.Equals(Position)
                && vert.UV.Equals(UV);
        }

        public override int GetHashCode()
        {
            var hashCode = -866678350;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + UV.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(VertexPositionTexture left, VertexPositionTexture right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionTexture left, VertexPositionTexture right)
        {
            return !(left == right);
        }

        internal static VertexPositionTexture Convert(Veldrid.Utilities.VertexPositionNormalTexture veldridVert)
        {
            return new VertexPositionTexture(veldridVert.Position, veldridVert.TextureCoordinates);
        }
    }
}
