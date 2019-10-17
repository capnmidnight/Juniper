using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            if(source is ICacheDestinationLayer dest)
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
        public Stream Open(ContentReference fileRef, IProgress prog)
        {
            Stream cached = null;
            foreach (var source in sources)
            {
                if (source.IsCached(fileRef))
                {
                    cached = source.Open(fileRef, prog);
                    break;
                }
            }

            return cached;
        }

        public Task<ResultType> Load<MediaTypeT, ResultType>(ContentReference fileRef, IDeserializer<ResultType, MediaTypeT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            if(fileRef == null)
            {
                throw new ArgumentNullException(nameof(fileRef));
            }

            if(deserializer == null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if(fileRef.ContentType != deserializer.ContentType)
            {
                throw new ArgumentException($"{nameof(fileRef)} parameter's content type ({fileRef.ContentType.Value}) does not match {nameof(deserializer)} parameter's content type ({deserializer.ContentType.Value})");
            }

            return Task.Run(() =>
            {
                var progs = prog.Split("Read", "Decode");
                var stream = Open(fileRef, progs[0]);
                if (stream == null)
                {
                    return default;
                }
                else
                {
                    return deserializer.Deserialize(stream, progs[1]);
                }
            });
        }

        public Task<ResultType> Load<MediaTypeT, ResultType>(ContentReference fileRef, IDeserializer<ResultType, MediaTypeT> deserializer)
            where MediaTypeT : MediaType
        {
            return Load(fileRef, deserializer, null);
        }

        public void Save<ResultType>(ContentReference fileRef, ResultType value, ISerializer<ResultType> serializer, IProgress prog)
        {
            using (var stream = Create(fileRef, true))
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public void Save<ResultType>(ContentReference fileRef, ResultType value, ISerializer<ResultType> serializer)
        {
            Save(fileRef, value, serializer, null);
        }

        public async Task Proxy(ContentReference fileRef, HttpListenerResponse response)
        {
            var stream = Open(fileRef, null);
            await stream.Proxy(response);
        }

        public Task Proxy<MediaTypeT>(ContentReference fileRef, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return Proxy(fileRef, context.Response);
        }

        public async Task<Stream> Open(StreamSource source, IProgress prog)
        {
            var stream = Open((ContentReference)source, prog);
            if (stream == null)
            {
                stream = await source.GetStream(prog);
            }
            return Cache(source, stream);
        }

        public Task<Stream> Open(StreamSource source)
        {
            return Open(source, null);
        }

        public async Task<ResultType> Load<ResultType, MediaTypeT>(StreamSource source, IDeserializer<ResultType, MediaTypeT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (deserializer == null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (source.ContentType != deserializer.ContentType)
            {
                throw new ArgumentException($"{nameof(source)} parameter's content type ({source.ContentType.Value}) does not match {nameof(deserializer)} parameter's content type ({deserializer.ContentType.Value})");
            }

            var progs = prog.Split("Read", "Decode");
            var stream = await Open(source, progs[0]);
            return deserializer.Deserialize(stream, progs[1]);

        }

        public Task<ResultType> Load<ResultType, MediaTypeT>(StreamSource source, IDeserializer<ResultType, MediaTypeT> deserializer)
            where MediaTypeT : MediaType
        {
            return Load(source, deserializer, null);
        }


        public async Task Proxy(StreamSource source, HttpListenerResponse response)
        {
            var stream = await Open(source);
            await stream.Proxy(response);
        }

        public Task Proxy(StreamSource source, HttpListenerContext context)
        {
            return Proxy(source, context.Response);
        }

        public IEnumerable<ContentReference> Get<MediaTypeT>(MediaTypeT ofType)
            where MediaTypeT : MediaType
        {
            foreach(var source in sources)
            {
                foreach(var fileRef in source.Get(ofType))
                {
                    yield return fileRef;
                }
            }
        }

        public bool Delete(ContentReference fileRef)
        {
            bool anyDelete = false;
            foreach(var dest in destinations)
            {
                anyDelete |= dest.Delete(fileRef);
            }

            return anyDelete;
        }
    }
}
