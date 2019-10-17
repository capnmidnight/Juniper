using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{
    public class MemoryCacheLayer : ICacheDestinationLayer
    {
        private readonly ConcurrentDictionary<MediaType, ConcurrentDictionary<string, MemoryStream>> store = new ConcurrentDictionary<MediaType, ConcurrentDictionary<string, MemoryStream>>();

        public virtual bool CanCache(ContentReference fileRef)
        {
            return true;
        }

        public bool IsCached(ContentReference fileRef)
        {
            return store.ContainsKey(fileRef.ContentType)
                && store[fileRef.ContentType].ContainsKey(fileRef.CacheID);
        }

        public Stream Create(ContentReference fileRef, bool overwrite)
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

        public Stream Cache(ContentReference fileRef, Stream stream)
        {
            var outStream = Create(fileRef, false);
            return new CachingStream(stream, outStream);
        }

        public Stream Open(ContentReference fileRef, IProgress prog)
        {
            Stream stream = null;
            if (IsCached(fileRef))
            {
                var data = store[fileRef.ContentType][fileRef.CacheID].ToArray();
                stream = new MemoryStream(data);

                if(prog != null)
                {
                    stream = new ProgressStream(stream, data.Length, prog);
                }
            }

            return stream;
        }

        public IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT contentType)
            where MediaTypeT : MediaType
        {
            if (store.ContainsKey(contentType))
            {
                foreach (var cacheID in store[contentType].Keys)
                {
                    yield return cacheID + contentType;
                }
            }
        }

        public bool Delete(ContentReference fileRef)
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
