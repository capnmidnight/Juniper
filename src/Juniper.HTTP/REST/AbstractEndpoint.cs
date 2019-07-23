using System.IO;
using System.Threading.Tasks;

using Juniper.Serialization;

namespace Juniper.HTTP.REST
{
    public class AbstractEndpoint
    {
        private readonly IDeserializer deserializer;
        internal readonly DirectoryInfo cacheLocation;

        protected AbstractEndpoint(IDeserializer deserializer, DirectoryInfo cacheLocation = null)
        {
            this.deserializer = deserializer;
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractEndpoint(IDeserializer deserializer, string cacheDirectoryName = null)
            : this(deserializer, cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }

        public bool IsCached<T, U>(AbstractRequest<T, U> search)
        {
            return search.IsCached(this);
        }

        public Task<U> Get<T, U>(AbstractRequest<T, U> search)
        {
            return search.Get(this);
        }

        public T DecodeObject<T>(Stream stream)
        {
            return deserializer.Deserialize<T>(stream);
        }
    }
}