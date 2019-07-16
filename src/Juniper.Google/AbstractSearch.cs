using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractSearch<T, U>
    {
        internal abstract bool IsCached(AbstractAPI api);

        internal abstract Task<U> Get(AbstractAPI api);
    }
}
