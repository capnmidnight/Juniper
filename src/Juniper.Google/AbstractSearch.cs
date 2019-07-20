using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractSearch<IntermediateType, ResultType>
    {
        internal abstract bool IsCached(AbstractAPI api);

        internal abstract Task<ResultType> Get(AbstractAPI api);
    }
}
