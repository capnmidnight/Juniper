using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public abstract class AbstractCacheContentReference<CacheLayerT, MediaTypeT> : IContentReference<MediaTypeT>
        where CacheLayerT : ICacheLayer
        where MediaTypeT : MediaType
    {
        protected readonly CacheLayerT layer;
        protected readonly IContentReference<MediaTypeT> fileRef;

        protected AbstractCacheContentReference(CacheLayerT layer, IContentReference<MediaTypeT> fileRef)
        {
            this.layer = layer;
            this.fileRef = fileRef;
        }

        public string CacheID
        {
            get
            {
                return fileRef.CacheID;
            }
        }

        public MediaTypeT ContentType
        {
            get
            {
                return fileRef.ContentType;
            }
        }

        protected abstract Task<Stream> InternalGetStream(IProgress prog);

        public async Task<Stream> GetStream(IProgress prog)
        {
            var stream = await InternalGetStream(prog);
            if (stream == null && fileRef is IStreamSource<MediaTypeT> streamSource)
            {
                stream = await streamSource.GetStream(prog);
                if (layer.CanCache(fileRef))
                {
                    stream = layer.Cache(fileRef, stream);
                }
            }
            return stream;
        }
    }
}
