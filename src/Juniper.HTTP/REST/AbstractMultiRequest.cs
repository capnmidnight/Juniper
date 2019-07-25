using System;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractMultiRequest<ResponseElementType, SubRequestType> : AbstractRequest<ResponseElementType, ResponseElementType[]>
        where SubRequestType : AbstractSingleRequest<ResponseElementType>
    {
        protected readonly SubRequestType[] subRequests;

        protected AbstractMultiRequest(int n, Func<SubRequestType> factory)
        {
            subRequests = new SubRequestType[n];
            for (var i = 0; i < subRequests.Length; ++i)
            {
                subRequests[i] = factory();
            }
        }

        public override bool IsCached(AbstractEndpoint api)
        {
            return subRequests.All(api.IsCached);
        }

        public override Task<ResponseElementType[]> Get(AbstractEndpoint api)
        {
            return Task.WhenAll(subRequests
                .Select(search => search.Get(api))
                .ToArray());
        }
    }
}