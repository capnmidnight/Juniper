using Veldrid;
using IndexT = System.UInt16;

namespace Juniper.VeldridIntegration
{
    public abstract class Mesh
    {
        public static Mesh<VertexT> OfQuads<VertexT>(VertexT[] vertices, IndexT[] indices)
            where VertexT : struct
        {
            return new Mesh<VertexT>(vertices.ToQuads(indices), vertices, indices);
        }

        public static Mesh<VertexT> OfTris<VertexT>(VertexT[] vertices, IndexT[] indices)
            where VertexT : struct
        {
            return new Mesh<VertexT>(vertices.ToTriangles(indices), vertices, indices);
        }

        internal abstract void Draw(CommandList commandList);
    }
}
