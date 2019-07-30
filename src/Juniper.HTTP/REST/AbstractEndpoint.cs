using System.IO;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public class AbstractEndpoint
    {
        internal readonly DirectoryInfo cacheLocation;

        protected AbstractEndpoint(DirectoryInfo cacheLocation = null)
        {
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractEndpoint(string cacheDirectoryName = null)
            : this(cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }

        public bool IsCached<T, U>(AbstractRequest<T, U> search)
        {
            return search.IsCached(this);
        }

        public Task<U> Get<T, U>(AbstractRequest<T, U> request)
        {
            return request.Get(this);
        }

        public Task<U> Post<T, U>(AbstractRequest<T, U> request)
        {
            return request.Post(this);
        }
    }
}