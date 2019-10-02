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

        public CachingStrategy AddLayer(ICacheLayer layer)
        {
            layers.Add(layer);
            return this;
        }

        public Stream Cache<MediaTypeT>(IContentReference<MediaTypeT> source, Stream stream)
            where MediaTypeT : MediaType
        {
            if (stream != null)
            {
                foreach (var layer in layers)
                {
                    if (layer.CanCache && !layer.IsCached(source))
                    {
                        stream = layer.Cache(source, stream);
                    }
                }
            }

            return stream;
        }

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

        public Stream OpenWrite<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            var stream = new ForkedStream();
            foreach (var layer in layers)
            {
                if (layer.CanCache && !layer.IsCached(source))
                {
                    stream.AddStream(layer.OpenWrite(source));
                }
            }

            return stream;
        }

        public void Copy<MediaTypeT>(IContentReference<MediaTypeT> source, FileInfo file)
            where MediaTypeT : MediaType
        {
            foreach (var layer in layers)
            {
                if (layer.CanCache)
                {
                    layer.Copy(source, file);
                }
            }
        }

        public bool IsCached<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            foreach (var layer in layers)
            {
                if (layer.IsCached(source))
                {
                    return true;
                }
            }

            return false;
        }

        public IStreamSource<MediaTypeT> GetCachedSource<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            IStreamSource<MediaTypeT> cached = null;
            foreach (var layer in layers)
            {
                if (layer.IsCached(source))
                {
                    cached = layer.GetCachedSource(source);
                    break;
                }
            }

            return cached;
        }

        public Task<Stream> Get<MediaTypeT>(IContentReference<MediaTypeT> source, IProgress prog)
            where MediaTypeT : MediaType
        {
            var cachedSource = GetCachedSource(source);
            return cachedSource.GetStream(prog);
        }

        public Task<Stream> Get<MediaTypeT>(IContentReference<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return Get(source, null);
        }

        public async Task<T> Decode<MediaTypeT, T>(IContentReference<MediaTypeT> source, IDeserializer<T> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var cachedSource = GetCachedSource(source);
            var stream = await cachedSource.GetStream(progs[0]);
            return stream.Decode(deserializer, progs[1]);

        }

        public Task<T> Decode<MediaTypeT, T>(IContentReference<MediaTypeT> source, IDeserializer<T> deserializer)
            where MediaTypeT : MediaType
        {
            return Decode(source, deserializer, null);
        }

        public async Task Proxy<MediaTypeT>(IContentReference<MediaTypeT> source, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            var stream = await Get(source);
            await stream.Proxy(response);
        }

        public Task Proxy<MediaTypeT>(IContentReference<MediaTypeT> source, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return Proxy(source, context.Response);
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
