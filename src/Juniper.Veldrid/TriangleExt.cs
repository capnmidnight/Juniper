using System;
using System.Collections.Generic;

using IndexT = System.UInt16;

namespace Juniper.VeldridIntegration
{
    public static class TriangleExt
    {
        public static Triangle<VertexT>[] ToTriangles<VertexT>(this VertexT[] verts, IndexT[] indices)
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

            var triangles = new List<Triangle<VertexT>>();
            for(var i = 0; i < indices.Length; i += 3)
            {
                triangles.Add(new Triangle<VertexT>(
                    verts[indices[i]],
                    verts[indices[i + 1]],
                    verts[indices[i + 2]]));
            }

            return triangles.ToArray();
        }
    }
}
