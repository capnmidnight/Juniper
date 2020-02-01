using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Juniper.IO
{
    /// <summary>
    /// A collection of source and destination layers for file content. Files
    /// originate from source layers and get cached in destination layers. Caching
    /// occurs automatically when a file is retrieved from a source.
    /// </summary>
    public class CachingStrategy : ICacheDestinationLayer
    {
        private readonly List<ICacheSourceLayer> sources = new List<ICacheSourceLayer>();
        private readonly List<ICacheDestinationLayer> destinations = new List<ICacheDestinationLayer>();

        /// <summary>
        /// Creates an empty caching strategy. Add cache layers to it with <see cref="AppendLayer(ICacheSourceLayer)"/>
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
            AppendLayer(new FileCacheLayer(cacheLocation));
        }

        /// <summary>
        /// Adds a layer to the caching strategy. Layers are checked in the order
        /// that they are added, so make sure to add the highest-latency cache
        /// layers last.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CachingStrategy AppendLayer(ICacheSourceLayer source)
        {
            sources.Add(source);
            if (source is ICacheDestinationLayer dest)
            {
                destinations.Add(dest);
            }

            return this;
        }

        /// <summary>
        /// Adds a layer to the caching strategy. Layers are checked in the order
        /// that they are added, so make sure to add the lowest-latency cache
        /// layers to the front of the list.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CachingStrategy PrependLayer(ICacheSourceLayer source)
        {
            sources.Insert(0, source);
            if (source is ICacheDestinationLayer dest)
            {
                destinations.Insert(0, dest);
            }

            return this;
        }

        /// <summary>
        /// Retrieve the first source layer of a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool GetSource<T>(out T layer)
            where T : ICacheSourceLayer
        {
            layer = default;
            foreach (var source in sources)
            {
                if (source is T t)
                {
                    layer = t;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieve the first destination layer of a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool GetDestination<T>(out T layer)
            where T : ICacheDestinationLayer
        {
            layer = default;
            foreach (var dest in destinations)
            {
                if (dest is T t)
                {
                    layer = t;
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
        public Stream Create(ContentReference fileRef)
        {
            var streams = new List<Stream>();

            foreach (var dest in destinations)
            {
                if (dest.CanCache(fileRef))
                {
                    streams.Add(dest.Create(fileRef));
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
            if (stream is object)
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
        public async Task<Stream> GetStreamAsync(ContentReference fileRef, IProgress prog)
        {
            Stream cached = null;
            foreach (var source in sources)
            {
                if (source.IsCached(fileRef))
                {
                    cached = await source
                        .GetStreamAsync(fileRef, prog)
                        .ConfigureAwait(false);
                    break;
                }
            }

            if (cached is null && fileRef is AbstractStreamSource fileSource)
            {
                cached = await fileSource
                    .GetStreamAsync(prog)
                    .ConfigureAwait(false);
                cached = Cache(fileRef, cached);
            }

            return cached;
        }

        /// <summary>
        /// Retrieve all the content references that match the given type within
        /// the cache layer.
        /// </summary>
        /// <param name="ofType"></param>
        /// <returns></returns>
        public IEnumerable<ContentReference> GetContentReferences(MediaType ofType)
        {
            foreach (var source in sources)
            {
                foreach (var fileRef in source.GetContentReferences(ofType))
                {
                    yield return fileRef;
                }
            }
        }

        /// <summary>
        /// Remove a file from all caching destinations.
        /// </summary>
        /// <param name="fileRef"></param>
        /// <returns>True, if any content was deleted. False, if the file was not found.</returns>
        public bool Delete(ContentReference fileRef)
        {
            var anyDelete = false;
            foreach (var dest in destinations)
            {
                anyDelete |= dest.Delete(fileRef);
            }

            return anyDelete;
        }
    }
}
