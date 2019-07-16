using System;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractMultiSearch<T, U> : AbstractSearch<T, T[]>
        where U : AbstractSingleSearch<T>
    {
        protected readonly U[] subSearches;

        protected AbstractMultiSearch(int n, Func<U> factory)
        {
            subSearches = new U[n];
            for (var i = 0; i < subSearches.Length; ++i)
            {
                subSearches[i] = factory();
            }
        }

        internal override bool IsCached(AbstractAPI api)
        {
            return subSearches.All(api.IsCached);
        }

        internal override Task<T[]> Get(AbstractAPI api)
        {
            return Task.WhenAll(subSearches
                .Select(search => search.Get(api))
                .ToArray());
        }
    }
}
