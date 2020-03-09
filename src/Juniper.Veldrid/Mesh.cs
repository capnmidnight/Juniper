using System;
using System.Collections.Generic;

using IndexT = System.UInt16;

namespace Juniper.VeldridIntegration
{
    public abstract class Mesh
    {
        public static Mesh<VertexT> OfQuads<VertexT>(VertexT[] vertices, IndexT[] indices)
            where VertexT : struct
        {
            if (vertices is null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            if (indices is null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            var quads = new List<Quad<VertexT>>();
            for (var i = 0; i < indices.Length; i += 4)
            {
                quads.Add(new Quad<VertexT>(
                    vertices[indices[i]],
                    vertices[indices[i + 1]],
                    vertices[indices[i + 2]],
                    vertices[indices[i + 3]]));
            }

            return new Mesh<VertexT>(
                faces: quads.ToArray(),
                vertices: vertices,
                indices: indices);
        }

        public static Mesh<VertexT> OfTris<VertexT>(VertexT[] vertices, IndexT[] indices)
            where VertexT : struct
        {
            if (vertices is null)
            {
                throw new ArgumentNullException(nameof(vertices));
            }

            if (indices is null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            var triangles = new List<Triangle<VertexT>>();
            for (var i = 0; i < indices.Length; i += 3)
            {
                triangles.Add(new Triangle<VertexT>(
                    vertices[indices[i]],
                    vertices[indices[i + 1]],
                    vertices[indices[i + 2]]));
            }

            return new Mesh<VertexT>(
                faces: triangles.ToArray(),
                vertices: vertices,
                indices: indices);
        }
    }
}
