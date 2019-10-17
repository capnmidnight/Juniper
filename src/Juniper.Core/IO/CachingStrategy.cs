using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public class CachingStrategy : ICacheDestinationLayer
    {
        private readonly List<ICacheSourceLayer> sources = new List<ICacheSourceLayer>();
        private readonly List<ICacheDestinationLayer> destinations = new List<ICacheDestinationLayer>();

        /// <summary>
        /// Creates an empty caching strategy. Add cache layers to it with <see cref="AddLayer(ICacheSourceLayer)"/>
        /// or no caching will occur.
        /// </summary>
        public CachingStrategy()
        { }

        /// <summary>
        /// Creates a caching strategy with a default cache layer that looks
        /// in a given directory for files.
        /// </summary>
        /// <param name="cacheLocation"></param>
        public CachingStrategy(DirectoryInfo cacheLocation)
        {
            AddLayer(new FileCacheLayer(cacheLocation));
        }

        /// <summary>
        /// Adds a layer to the caching strategy. Layers are checked in the order
        /// that they are added.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CachingStrategy AddLayer(ICacheSourceLayer source)
        {
            sources.Add(source);
            if (source is ICacheDestinationLayer dest)
            {
                destinations.Add(dest);
            }
            return this;
        }

        public bool GetSource<T>(out T layer)
            where T : ICacheSourceLayer
        {
            layer = default;
            foreach (var source in sources)
            {
                if (source is T)
                {
                    layer = (T)source;
                    return true;
                }
            }

            return false;
        }

        public bool GetDestination<T>(out T layer)
            where T : ICacheDestinationLayer
        {
            layer = default;
            foreach (var dest in destinations)
            {
                if (dest is T)
                {
                    layer = (T)dest;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a stream that will write to all of the cache layers that support
        /// writing streams.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public Stream Create(ContentReference fileRef, bool overwrite)
        {
            var streams = new List<Stream>();

            foreach (var dest in destinations)
            {
                if (dest.CanCache(fileRef)
                    && (overwrite || !dest.IsCached(fileRef)))
                {
                    streams.Add(dest.Create(fileRef, overwrite));
                }
            }

            if (streams.Count == 0)
            {
                return null;
            }
            else
            {
                return new ForkedStream(streams.ToArray());
            }
        }

        /// <summary>
        /// Searches the cache layers in sequence for the file.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <param name="stream"></param>
        /// <returns>null if the file does not exist in any of the cache layers.</returns>
        public Stream Cache(ContentReference fileRef, Stream stream)
        {
            if (stream != null)
            {
                foreach (var dest in destinations)
                {
                    if (dest.CanCache(fileRef)
                        && !dest.IsCached(fileRef))
                    {
                        stream = dest.Cache(fileRef, stream);
                    }
                }
            }

            return stream;
        }

        /// <summary>
        /// Returns true if any of the cache layers support writing streams.
        /// Currently, only the <see cref="FileCacheLayer"/> supports writing
        /// streams.
        /// </summary>
        public bool CanCache(ContentReference fileRef)
        {
            foreach (var dest in destinations)
            {
                if (dest.CanCache(fileRef))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if any of the cache layers contain the referenced file.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns>False, if none of the cache layers contain the file</returns>
        public bool IsCached(ContentReference fileRef)
        {
            foreach (var source in sources)
            {
                if (source.IsCached(fileRef))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a stream reader reference for the first cache layer that contains
        /// the referenced file.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns>Null, if the file does not exist in the cache</returns>
        public async Task<Stream> Open(ContentReference fileRef, IProgress prog)
        {
            Stream cached = null;
            foreach (var source in sources)
            {
                if (source.IsCached(fileRef))
                {
                    cached = await source.Open(fileRef, prog);
                    break;
                }
            }

            if (cached == null && fileRef is StreamSource fileSource)
            {
                cached = await fileSource.GetStream(prog);
            }

            return cached;
        }

        /// <summary>
        /// Retrieve all the content references that match the given type within
        /// the cache layer.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="ofType"></param>
        /// <returns></returns>
        public IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType
        {
            foreach (var source in sources)
            {
                foreach (var fileRef in source.Get(ofType))
                {
                    yield return fileRef;
                }
            }
        }

        public bool Delete(ContentReference fileRef)
        {
            bool anyDelete = false;
            foreach (var dest in destinations)
            {
                anyDelete |= dest.Delete(fileRef);
            }

            return anyDelete;
        }
    }
}
