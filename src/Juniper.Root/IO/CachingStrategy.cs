using Juniper.Progress;

using System.Collections;

namespace Juniper.IO
{
    /// <summary>
    /// A collection of source and destination layers for file content. Files
    /// originate from source layers and get cached in destination layers. Caching
    /// occurs automatically when a file is retrieved from a source.
    /// </summary>
    public sealed class CachingStrategy :
        ICacheDestinationLayer,
        IEnumerable<ICacheSourceLayer>
    {
        /// <summary>
        /// Creates a caching strategy that uses the user's temp directory.
        /// </summary>
        /// <returns></returns>
        public static CachingStrategy GetTempCache()
        {
            return new CachingStrategy
            {
                FileCacheLayer.GetTempCache()
            };
        }

        public static CachingStrategy GetNoCache()
        {
            return new CachingStrategy();
        }

        private readonly List<ICacheSourceLayer> sources = new();
        private readonly List<ICacheDestinationLayer> destinations = new();

        /// <summary>
        /// Creates an empty caching strategy. Add cache layers to it with <see cref="AppendLayer(ICacheSourceLayer)"/>
        /// or no caching will occur.
        /// </summary>
        public CachingStrategy()
        {
            sources.Add(new StreamSourceLayer(this));
        }

        /// <summary>
        /// Adds a layer to the caching strategy. Layers are checked in the order
        /// that they are added, so make sure to add the highest-latency cache
        /// layers last.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public void Add(ICacheSourceLayer source)
        {
            sources.Insert(sources.Count - 1, source);

            if (source is ICacheDestinationLayer dest)
            {
                destinations.Add(dest);
            }
        }

        /// <summary>
        /// Adds a layer to the caching strategy. Layers are checked in the order
        /// that they are added, so make sure to add the lowest-latency cache
        /// layers to the front of the list.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public void Prepend(ICacheSourceLayer source)
        {
            sources.Insert(0, source);

            if (source is ICacheDestinationLayer dest)
            {
                destinations.Insert(0, dest);
            }
        }

        /// <summary>
        /// Adds a layer to the caching strategy. Layers are checked in the order
        /// that they are added, so make sure to add the highest-latency cache
        /// layers last.
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public void AddBackup(ICacheDestinationLayer dest)
        {
            sources.Insert(sources.Count - 1, dest);
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
        /// Creates a stream that will write to the first primary cache layer that supports
        /// writing the given content reference, and any secondary cache layers as well.
        /// </summary>
        /// <exception cref="InvalidOperationException">If no primary cache layer could be found.</exception>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public Stream Create(ContentReference fileRef)
        {
            foreach (var dest in destinations)
            {
                if (dest.CanCache(fileRef))
                {
                    return dest.Create(fileRef);
                }
            }

            throw new InvalidOperationException("Could not cache the file " + fileRef);
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
                if (source is not StreamSourceLayer
                    && source.IsCached(fileRef))
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
            if (fileRef is null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            foreach (var source in sources)
            {
                if (source.IsCached(fileRef))
                {
                    return await source
                        .GetStreamAsync(fileRef, prog)
                        .ConfigureAwait(false);
                }
            }

            throw new FileNotFoundException("Could not find file " + fileRef, fileRef.FileName);
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

        public IEnumerator<ICacheSourceLayer> GetEnumerator()
        {
            return ((IEnumerable<ICacheSourceLayer>)destinations).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ICacheSourceLayer>)destinations).GetEnumerator();
        }
    }
}
