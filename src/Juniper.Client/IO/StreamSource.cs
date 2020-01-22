using System.IO;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public abstract class StreamSource : ContentReference
    {
        protected StreamSource(MediaType contentType)
            : base(contentType)
        { }

        protected StreamSource(string cacheID, MediaType contentType)
            : base(cacheID, contentType)
        { }

        public abstract Task<Stream> GetStreamAsync(IProgress prog = null);
    }
}
