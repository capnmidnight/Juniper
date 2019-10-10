using System.Collections.Concurrent;
using System.IO;

namespace Juniper.IO
{
    public class MemoryCacheLayer : ICacheDestinationLayer
    {
        private readonly ConcurrentDictionary<string, MemoryStream> store = new ConcurrentDictionary<string, MemoryStream>();

        private static string MakeKey<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return fileRef.ContentType.Value + "/" + fileRef.CacheID;
        }

        public virtual bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return true;
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var key = MakeKey(fileRef);
            return store.ContainsKey(key);
        }

        public Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var key = MakeKey(fileRef);
            var mem = new MemoryStream();
            store[key] = mem;
            return mem;
        }

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType
        {
            var outStream = Create(fileRef);
            return new CachingStream(stream, outStream);
        }

        public Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var key = MakeKey(fileRef);
            return new MemoryStream(store[key].ToArray());
        }
    }
}
