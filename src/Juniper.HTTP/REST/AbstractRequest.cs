using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractRequest<IntermediateType, ResponseType>
    {
        public abstract bool IsCached(AbstractEndpoint api);

        public abstract Task<ResponseType> Get(AbstractEndpoint api);

        public abstract Task<ResponseType> Post(AbstractEndpoint api);
    }
}