using System.Collections.Generic;

namespace Juniper.VeldridIntegration
{
    public class Quad<VertexT>
        : IFace<VertexT>
        where VertexT : struct
    {
        public VertexT A { get; }
        public VertexT B { get; }
        public VertexT C { get; }
        public VertexT D { get; }

        public int ElementCount => 4;

        public IEnumerable<VertexT> Elements
        {
            get
            {
                yield return A;
                yield return B;
                yield return C;
                yield return D;
            }
        }

        public Quad(VertexT a, VertexT b, VertexT c, VertexT d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }
}
