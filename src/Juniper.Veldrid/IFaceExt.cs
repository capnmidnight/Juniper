using System;
using System.Collections.Generic;

using IndexT = System.UInt16;

namespace Juniper.VeldridIntegration
{
    public static class IFaceExt
    {
        private static void AddVert<VertexT>(this List<VertexT> verts, VertexT vert, IndexT[] indices, int t, int index)
            where VertexT : struct
        {
            var indexA = verts.IndexOf(vert);
            if (indexA == -1)
            {
                indexA = verts.Count;
                verts.Add(vert);
            }
            indices[t + index] = (IndexT)indexA;
        }

        private static (VertexT[] verts, IndexT[] indices) ToVertsShort_Internal<VertexT>(params IFace<VertexT>[] faces)
            where VertexT : struct
        {
            var verts = new List<VertexT>();
            if (faces.Length == 0)
            {
                return (Array.Empty<VertexT>(), Array.Empty<IndexT>());
            }

            var first = faces[0];
            var elementCount = first.ElementCount;
            var indices = new IndexT[faces.Length * elementCount];
            for (var i = 0; i < faces.Length; ++i)
            {
                var face = faces[i];
                var t = i * face.ElementCount;
                var j = 0;
                foreach (var element in face.Elements)
                {
                    verts.AddVert(element, indices, t, j++);
                }
            }

            return (verts.ToArray(), indices);
        }

        public static (VertexT[] verts, IndexT[] indices) ToVerts<VertexT>(this IFace<VertexT>[] faces)
            where VertexT : struct
        {
            if (faces is null)
            {
                throw new ArgumentNullException(nameof(faces));
            }

            return ToVertsShort_Internal(faces);
        }

        public static (VertexT[] verts, IndexT[] indices) ToVertsShort<VertexT>(this IFace<VertexT> face)
            where VertexT : struct
        {
            if (face is null)
            {
                throw new ArgumentNullException(nameof(face));
            }

            return ToVertsShort_Internal(face);
        }
    }
}
