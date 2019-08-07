using Juniper.Json;
using Juniper.Serialization;

namespace Juniper.HTTP.REST
{
    public abstract class AbstractJsonRequest<T> : AbstractSingleRequest<T>
    {
        protected AbstractJsonRequest(AbstractRequestConfiguration api, string path, string cacheSubDirectoryName)
            : base(api, new JsonFactory().Specialize<T>(), path, cacheSubDirectoryName)
        {
            SetContentType("application/json", "json");
        }
    }
}