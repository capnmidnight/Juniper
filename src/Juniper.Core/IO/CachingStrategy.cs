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

        /// <summary>
        /// Creates a stream that will write to all of the cache layers that support
        /// writing streams.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public Stream Create<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var stream = new ForkedStream();
            foreach (var dest in destinations)
            {
                if (dest.CanCache(fileRef)
                    && !dest.IsCached(fileRef))
                {
                    stream.AddStream(dest.Create(fileRef));
                }
            }

            return stream;
        }

        /// <summary>
        /// Searches the cache layers in sequence for the file.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <param name="stream"></param>
        /// <returns>null if the file does not exist in any of the cache layers.</returns>
        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> fileRef, Stream stream)
            where MediaTypeT : MediaType
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
        public bool CanCache<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
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
        /// Copies a file to all of the cache layers that support writing streams.
        /// </summary>
        /// <remarks>
        /// If none of the cache layers support writing, then no copy operation
        /// takes place.
        /// </remarks>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <param name="file"></param>
        public void Copy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, FileInfo file)
            where MediaTypeT : MediaType
        {
            foreach (var dest in destinations)
            {
                if (dest.CanCache(fileRef))
                {
                    dest.Copy(fileRef, file);
                }
            }
        }

        /// <summary>
        /// Checks to see if any of the cache layers contain the referenced file.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns>False, if none of the cache layers contain the file</returns>
        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
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
        public Stream Open<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            Stream cached = null;
            foreach (var source in sources)
            {
                if (source.IsCached(fileRef))
                {
                    cached = source.Open(fileRef);
                    break;
                }
            }

            return cached;
        }

        public Task<ResultType> Load<MediaTypeT, ResultType>(IContentReference<MediaTypeT> fileRef, IDeserializer<ResultType, MediaTypeT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var stream = Open(fileRef);
            if (stream == null)
            {
                return Task.FromResult(default(ResultType));
            }
            else
            {
                return deserializer.DeserializeAsync(stream, progs[1]);
            }
        }

        public Task<ResultType> Load<MediaTypeT, ResultType>(IContentReference<MediaTypeT> fileRef, IDeserializer<ResultType, MediaTypeT> deserializer)
            where MediaTypeT : MediaType
        {
            return Load(fileRef, deserializer, null);
        }

        public void Save<MediaTypeT, ResultType>(IContentReference<MediaTypeT> fileRef, ResultType value, ISerializer<ResultType> serializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            using (var stream = Create(fileRef))
            {
                serializer.Serialize(stream, value, prog);
            }
        }

        public void Save<MediaTypeT, ResultType>(IContentReference<MediaTypeT> fileRef, ResultType value, ISerializer<ResultType> serializer)
            where MediaTypeT : MediaType
        {
            Save(fileRef, value, serializer, null);
        }

        public async Task Proxy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            var stream = Open(fileRef);
            await stream.Proxy(response);
        }

        public Task Proxy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return Proxy(fileRef, context.Response);
        }

        public async Task<Stream> Open<MediaTypeT>(IStreamSource<MediaTypeT> source, IProgress prog)
            where MediaTypeT : MediaType
        {
            var stream = Open((IContentReference<MediaTypeT>)source);
            if (stream == null)
            {
                stream = await source.GetStream(prog);
            }
            return Cache(source, stream);
        }

        public Task<Stream> Open<MediaTypeT>(IStreamSource<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return Open(source, null);
        }

        public async Task<ResultType> Load<MediaTypeT, ResultType>(IStreamSource<MediaTypeT> source, IDeserializer<ResultType, MediaTypeT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var stream = await Open(source, progs[0]);
            return deserializer.Deserialize(stream, progs[1]);

        }

        public Task<ResultType> Load<MediaTypeT, ResultType>(IStreamSource<MediaTypeT> source, IDeserializer<ResultType, MediaTypeT> deserializer)
            where MediaTypeT : MediaType
        {
            return Load(source, deserializer, null);
        }


        public async Task Proxy<MediaTypeT>(IStreamSource<MediaTypeT> source, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            var stream = await Open(source);
            await stream.Proxy(response);
        }

        public Task Proxy<MediaTypeT>(IStreamSource<MediaTypeT> source, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return Proxy(source, context.Response);
        }
    }
}
