using System;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractRequest<IntermediateType, ResponseType>
    {
        protected readonly AbstractEndpoint api;

        protected AbstractRequest(AbstractEndpoint api)
        {
            this.api = api;
        }

        public abstract bool IsCached { get; }

        public virtual Task<ResponseType> Get()
        {
            throw new NotImplementedException();
        }

        public virtual Task<ResponseType> Post()
        {
            throw new NotImplementedException();
        }
    }
}