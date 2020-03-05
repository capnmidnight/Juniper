using System;
using System.Collections.Generic;

namespace Juniper.VeldridIntegration
{
    public class Triangle<VertexT> where VertexT : struct
    {
        public VertexT A { get; }
        public VertexT B { get; }
        public VertexT C { get; }

        public Triangle(VertexT a, VertexT b, VertexT c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public static class TriangleExt
    {
        public static (VertexT[] verts, ushort[] indices) ToVertsShort<VertexT>(this Triangle<VertexT>[] triangles)
            where VertexT : struct
        {
            if (triangles is null)
            {
                throw new ArgumentNullException(nameof(triangles));
            }
            var verts = new List<VertexT>();
            var indices = new ushort[triangles.Length * 3];
            for(var i = 0; i < triangles.Length; ++i)
            {
                var t = i * 3;
                var triangle = triangles[i];
                AddVert(verts, triangle.A, indices, t, 0);
                AddVert(verts, triangle.B, indices, t, 1);
                AddVert(verts, triangle.C, indices, t, 2);
            }

            return (verts.ToArray(), indices);
        }

        private static void AddVert<VertexT>(List<VertexT> verts, VertexT vert, ushort[] indices, int t, int index) where VertexT : struct
        {
            var indexA = verts.IndexOf(vert);
            if (indexA == -1)
            {
                indexA = verts.Count;
                verts.Add(vert);
            }
            indices[t + index] = (ushort)indexA;
        }

        public static Triangle<VertexT> ToTriangles<VertexT>(this VertexT[] verts, uint[] indices)
            where VertexT : struct
        {
            throw new NotImplementedException();
        }
    }
}
