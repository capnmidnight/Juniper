using System;
using System.Collections.Generic;

using Veldrid;
using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public static class VertexTypeCache
    {
        private static readonly Dictionary<Type, VertexLayoutDescription> cache = new Dictionary<Type, VertexLayoutDescription>
        {
            [typeof(VertexPosition)] = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3)),

            [typeof(VertexPositionTexture)] = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                new VertexElementDescription("TextureCoordinates", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)),

            [typeof(VertexPositionNormalTexture)] = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                new VertexElementDescription("Normal", VertexElementSemantic.Normal, VertexElementFormat.Float3),
                new VertexElementDescription("TextureCoordinates", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
        };

        public static VertexLayoutDescription GetDescription<VertexT>()
            where VertexT : struct
        {
            return cache[typeof(VertexT)];
        }
    }
}
