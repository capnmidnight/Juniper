using System;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractMultiRequest<ResponseElementType, SubRequestType> : AbstractRequest<ResponseElementType, ResponseElementType[]>
        where SubRequestType : AbstractSingleRequest<ResponseElementType>
    {
        protected readonly SubRequestType[] subRequests;

        protected AbstractMultiRequest(AbstractEndpoint api, int n, Func<SubRequestType> factory)
            : base(api)

        {
            subRequests = new SubRequestType[n];
            for (var i = 0; i < subRequests.Length; ++i)
            {
                subRequests[i] = factory();
            }
        }

        public override bool IsCached
        {
            get
            {
                return subRequests.All(s => s.IsCached);
            }
        }

        public override Task<ResponseElementType[]> Get()
        {
            return Task.WhenAll(subRequests
                .Select(search => search.Get())
                .ToArray());
        }

        public override Task<ResponseElementType[]> Post()
        {
            return Task.WhenAll(subRequests
                .Select(search => search.Post())
                .ToArray());
        }
    }
}