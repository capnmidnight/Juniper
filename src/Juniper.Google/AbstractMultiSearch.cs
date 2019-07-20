using System;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.Google
{
    public abstract class AbstractMultiSearch<ResultElementType, SubSearchType> : AbstractSearch<ResultElementType, ResultElementType[]>
        where SubSearchType : AbstractSingleSearch<ResultElementType>
    {
        protected readonly SubSearchType[] subSearches;

        protected AbstractMultiSearch(int n, Func<SubSearchType> factory)
        {
            subSearches = new SubSearchType[n];
            for (var i = 0; i < subSearches.Length; ++i)
            {
                subSearches[i] = factory();
            }
        }

        internal override bool IsCached(AbstractAPI api)
        {
            return subSearches.All(api.IsCached);
        }

        internal override Task<ResultElementType[]> Get(AbstractAPI api)
        {
            return Task.WhenAll(subSearches
                .Select(search => search.Get(api))
                .ToArray());
        }
    }
}
