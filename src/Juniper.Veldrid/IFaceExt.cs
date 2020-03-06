using System;
using System.Collections.Generic;

namespace Juniper.VeldridIntegration
{
    public static class IFaceExt
    {
        private static void AddVert<VertexT>(this List<VertexT> verts, VertexT vert, ushort[] indices, int t, int index)
            where VertexT : struct
        {
            var indexA = verts.IndexOf(vert);
            if (indexA == -1)
            {
                indexA = verts.Count;
                verts.Add(vert);
            }
            indices[t + index] = (ushort)indexA;
        }

        private static (VertexT[] verts, ushort[] indices) ToVertsShort_Internal<VertexT>(params IFace<VertexT>[] faces)
            where VertexT : struct
        {
            var verts = new List<VertexT>();
            if(faces.Length == 0)
            {
                return (Array.Empty<VertexT>(), Array.Empty<ushort>());
            }

            var first = faces[0];
            var elementCount = first.ElementCount;
            var indices = new ushort[faces.Length * elementCount];
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

        public static (VertexT[] verts, ushort[] indices) ToVertsShort<VertexT>(this IFace<VertexT>[] faces)
            where VertexT : struct
        {
            if (faces is null)
            {
                throw new ArgumentNullException(nameof(faces));
            }

            return ToVertsShort_Internal(faces);
        }

        public static (VertexT[] verts, ushort[] indices) ToVertsShort<VertexT>(this IFace<VertexT> face)
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
