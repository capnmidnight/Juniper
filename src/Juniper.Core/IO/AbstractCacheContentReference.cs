using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public abstract class AbstractCacheContentReference<CacheLayerT, MediaTypeT> : IContentReference<MediaTypeT>
        where CacheLayerT : ICacheLayer
        where MediaTypeT : MediaType
    {
        protected readonly CacheLayerT layer;
        protected readonly IContentReference<MediaTypeT> source;

        protected AbstractCacheContentReference(CacheLayerT layer, IContentReference<MediaTypeT> source)
        {
            this.layer = layer;
            this.source = source;
        }

        public string CacheID
        {
            get
            {
                return source.CacheID;
            }
        }

        public MediaTypeT ContentType
        {
            get
            {
                return source.ContentType;
            }
        }

        protected abstract Task<Stream> InternalGetStream(IProgress prog);

        public async Task<Stream> GetStream(IProgress prog)
        {
            var stream = await InternalGetStream(prog);
            if (stream == null && source is IStreamSource<MediaTypeT> streamSource)
            {
                stream = await streamSource.GetStream(prog);
                if (layer.CanCache)
                {
                    stream = layer.Cache(source, stream);
                }
            }
            return stream;
        }
    }
}
