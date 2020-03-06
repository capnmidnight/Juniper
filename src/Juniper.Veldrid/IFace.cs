using System.Collections.Generic;

namespace Juniper.VeldridIntegration
{
    public interface IFace<out VertexT>
        where VertexT : struct
    {
        public int ElementCount { get; }

        public IEnumerable<VertexT> Elements { get; }
    }
}
