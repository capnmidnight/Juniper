using System;
using System.Numerics;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public struct VertexPositionNormalTexture : IEquatable<VertexPositionNormalTexture>
    {
        [VertexElement(VertexElementSemantic.Position)]
        public Vector3 Position { get; }

        [VertexElement(VertexElementSemantic.Normal)]
        public Vector3 Normal { get; }

        [VertexElement(VertexElementSemantic.TextureCoordinate)]
        public Vector2 UV { get; }

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
        }

        public override bool Equals(object obj)
        {
            return obj is VertexPositionNormalTexture vert
                && Equals(vert);
        }

        public bool Equals(VertexPositionNormalTexture vert)
        {
            return vert.Position.Equals(Position)
                && vert.Normal.Equals(Normal)
                && vert.UV.Equals(UV);
        }

        public override int GetHashCode()
        {
            var hashCode = -866678350;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Normal.GetHashCode();
            hashCode = hashCode * -1521134295 + UV.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
        {
            return !(left == right);
        }

        internal static VertexPositionNormalTexture Convert(Veldrid.Utilities.VertexPositionNormalTexture veldridVert)
        {
            return new VertexPositionNormalTexture(veldridVert.Position, veldridVert.Normal, veldridVert.TextureCoordinates);
        }
    }
}
