using System;
using System.Collections.Generic;

namespace Juniper.VeldridIntegration
{
    public static class QuadExt
    {
        public static Quad<VertexT>[] ToQuads<VertexT>(this VertexT[] verts, uint[] indices)
            where VertexT : struct
        {
            if (verts is null)
            {
                throw new ArgumentNullException(nameof(verts));
            }

            if (indices is null)
            {
                throw new ArgumentNullException(nameof(indices));
            }

            var quads = new List<Quad<VertexT>>();
            for (var i = 0; i < indices.Length; i += 4)
            {
                quads.Add(new Quad<VertexT>(
                    verts[indices[i]],
                    verts[indices[i + 1]],
                    verts[indices[i + 2]],
                    verts[indices[i + 3]]));
            }

            return quads.ToArray();
        }
    }
}
