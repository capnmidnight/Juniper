using System;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractMultiRequest<ResponseElementType, SubRequestType> : AbstractRequest<ResponseElementType, ResponseElementType[]>
        where SubRequestType : AbstractSingleRequest<ResponseElementType>
    {
        protected readonly SubRequestType[] subSearches;

        protected AbstractMultiRequest(int n, Func<SubRequestType> factory)
        {
            subSearches = new SubRequestType[n];
            for (var i = 0; i < subSearches.Length; ++i)
            {
                subSearches[i] = factory();
            }
        }

        public override bool IsCached(AbstractEndpoint api)
        {
            return subSearches.All(api.IsCached);
        }

        public override Task<ResponseElementType[]> Get(AbstractEndpoint api)
        {
            return Task.WhenAll(subSearches
                .Select(search => search.Get(api))
                .ToArray());
        }
    }
}
