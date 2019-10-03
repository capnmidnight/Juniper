using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public class CachingStrategy : ICacheLayer
    {
        private readonly List<ICacheLayer> layers = new List<ICacheLayer>();

        /// <summary>
        /// Creates an empty caching strategy. Add cache layers to it with <see cref="AddLayer(ICacheLayer)"/>
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
        /// <param name="layer"></param>
        /// <returns></returns>
        public CachingStrategy AddLayer(ICacheLayer layer)
        {
            layers.Add(layer);
            return this;
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
                foreach (var layer in layers)
                {
                    if (layer.CanCache && !layer.IsCached(fileRef))
                    {
                        stream = layer.Cache(fileRef, stream);
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
        public bool CanCache
        {
            get
            {
                foreach (var layer in layers)
                {
                    if (layer.CanCache)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Creates a stream that will write to all of the cache layers that support
        /// writing streams.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <returns></returns>
        public Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            var stream = new ForkedStream();
            foreach (var layer in layers)
            {
                if (layer.CanCache && !layer.IsCached(fileRef))
                {
                    stream.AddStream(layer.OpenWrite(fileRef));
                }
            }

            return stream;
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
            foreach (var layer in layers)
            {
                if (layer.CanCache)
                {
                    layer.Copy(fileRef, file);
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
            foreach (var layer in layers)
            {
                if (layer.IsCached(fileRef))
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
        public IStreamSource<MediaTypeT> GetCachedSource<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            IStreamSource<MediaTypeT> cached = null;
            foreach (var layer in layers)
            {
                if (layer.IsCached(fileRef))
                {
                    cached = layer.GetCachedSource(fileRef);
                    break;
                }
            }

            return cached;
        }

        /// <summary>
        /// Searches for the first cache layer that contains the referenced file
        /// and opens the reader stream from the cache.
        /// </summary>
        /// <typeparam name="MediaTypeT"></typeparam>
        /// <param name="fileRef"></param>
        /// <param name="prog"></param>
        /// <returns>Null, if the file does not exist in the cache</returns>
        public Task<Stream> Get<MediaTypeT>(IContentReference<MediaTypeT> fileRef, IProgress prog)
            where MediaTypeT : MediaType
        {
            var cachedSource = GetCachedSource(fileRef);
            return cachedSource.GetStream(prog);
        }

        public Task<Stream> Get<MediaTypeT>(IContentReference<MediaTypeT> fileRef)
            where MediaTypeT : MediaType
        {
            return Get(fileRef, null);
        }

        public async Task<T> Decode<MediaTypeT, T>(IContentReference<MediaTypeT> fileRef, IDeserializer<T> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var cachedSource = GetCachedSource(fileRef);
            var stream = await cachedSource.GetStream(progs[0]);
            return stream.Decode(deserializer, progs[1]);

        }

        public Task<T> Decode<MediaTypeT, T>(IContentReference<MediaTypeT> fileRef, IDeserializer<T> deserializer)
            where MediaTypeT : MediaType
        {
            return Decode(fileRef, deserializer, null);
        }

        public async Task Proxy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            var stream = await Get(fileRef);
            await stream.Proxy(response);
        }

        public Task Proxy<MediaTypeT>(IContentReference<MediaTypeT> fileRef, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return Proxy(fileRef, context.Response);
        }

        public async Task<Stream> Get<MediaTypeT>(IStreamSource<MediaTypeT> source, IProgress prog)
            where MediaTypeT : MediaType
        {
            var bestSource = GetCachedSource(source) ?? source;
            var stream = await bestSource.GetStream(prog);
            return Cache(source, stream);
        }

        public Task<Stream> Get<MediaTypeT>(IStreamSource<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return Get(source, null);
        }

        public async Task<T> Decode<MediaTypeT, T>(IStreamSource<MediaTypeT> source, IDeserializer<T> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var bestSource = GetCachedSource(source) ?? source;
            var stream = await bestSource.GetStream(progs[0]);
            return Cache(source, stream)
                .Decode(deserializer, progs[1]);

        }

        public Task<T> Decode<MediaTypeT, T>(IStreamSource<MediaTypeT> source, IDeserializer<T> deserializer)
            where MediaTypeT : MediaType
        {
            return Decode(source, deserializer, null);
        }


        public async Task Proxy<MediaTypeT>(IStreamSource<MediaTypeT> source, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            var stream = await Get(source);
            await stream.Proxy(response);
        }

        public Task Proxy<MediaTypeT>(IStreamSource<MediaTypeT> source, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return Proxy(source, context.Response);
        }
    }
}
