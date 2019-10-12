using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Juniper.IO
{
    public class MemoryCacheLayer : ICacheDestinationLayer
    {
        private readonly ConcurrentDictionary<MediaType, ConcurrentDictionary<string, MemoryStream>> store = new ConcurrentDictionary<MediaType, ConcurrentDictionary<string, MemoryStream>>();

        public virtual bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return true;
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return store.ContainsKey(fileRef.ContentType)
                && store[fileRef.ContentType].ContainsKey(fileRef.CacheID);
        }

        public Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef, bool overwrite)
            where MediaTypeT : MediaType
        {
            Stream stream = null;
            if (!store.ContainsKey(fileRef.ContentType))
            {
                store[fileRef.ContentType] = new ConcurrentDictionary<string, MemoryStream>();
            }

            var subStore = store[fileRef.ContentType];

            if (overwrite || !subStore.ContainsKey(fileRef.CacheID))
            {
                var mem = new MemoryStream();
                stream = subStore[fileRef.CacheID] = mem;
            }

            return stream;
        }

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType
        {
            var outStream = Create(fileRef, false);
            return new CachingStream(stream, outStream);
        }

        public Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            if (!IsCached(fileRef))
            {
                return null;
            }
            else
            {
                return new MemoryStream(store[fileRef.ContentType][fileRef.CacheID].ToArray());
            }
        }

        public IEnumerable<IContentReference<MediaTypeT>> Get<MediaTypeT>(MediaTypeT contentType)
            where MediaTypeT : MediaType
        {
            if (store.ContainsKey(contentType))
            {
                foreach (var cacheID in store[contentType].Keys)
                {
                    yield return new ContentReference<MediaTypeT>(cacheID, contentType);
                }
            }
        }

        public bool Delete<MediaTypeT>(IContentReference<MediaTypeT> fileRef) where MediaTypeT : MediaType
        {
            if (IsCached(fileRef))
            {
                return store[fileRef.ContentType]
                    .TryRemove(fileRef.CacheID, out _);
            }
            else
            {
                return false;
            }
        }
    }
}
