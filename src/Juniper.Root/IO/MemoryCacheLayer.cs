using Juniper.Progress;

using System.Collections.Concurrent;

namespace Juniper.IO
{
    public class MemoryCacheLayer : ICacheDestinationLayer
    {
        private readonly ConcurrentDictionary<MediaType, ConcurrentDictionary<string, MemoryStream>> store = new();

        public virtual bool CanCache(ContentReference fileRef)
        {
            return true;
        }

        public virtual bool IsCached(ContentReference fileRef)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            return fileRef.CacheID is not null
                && store.ContainsKey(fileRef.ContentType)
                && store[fileRef.ContentType].ContainsKey(fileRef.CacheID);
        }

        public Stream Create(ContentReference fileRef)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            var subStore = store.Default(fileRef.ContentType);
            var mem = new MemoryStream();
            return subStore[fileRef.CacheID] = mem;
        }

        public Task<Stream?> GetStreamAsync(ContentReference fileRef, IProgress? prog = null)
        {
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if (!IsCached(fileRef))
            {
                throw new FileNotFoundException("File not found in memory cache!", fileRef.ToString());
            }

            var data = store[fileRef.ContentType][fileRef.CacheID!].ToArray();
            var stream = new MemoryStream(data);
            var progStream = new ProgressStream(stream, data.Length, prog, true);
            return Task.FromResult((Stream?)progStream);
        }

        public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
        {
            if (store.ContainsKey(ofType))
            {
                foreach (var cacheID in store[ofType].Keys)
                {
                    yield return cacheID + ofType;
                }
            }
        }

        public bool Delete(ContentReference fileRef)
        {
            if (fileRef is null)
            {
                throw new System.ArgumentNullException(nameof(fileRef));
            }

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
