using System;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractRequest<IntermediateType, ResponseType>
    {
        public abstract bool IsCached(AbstractEndpoint api);

        public virtual Task<ResponseType> Get(AbstractEndpoint api)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ResponseType> Post(AbstractEndpoint api)
        {
            throw new NotImplementedException();
        }
    }
}