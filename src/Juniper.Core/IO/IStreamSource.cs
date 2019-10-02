using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public interface IContentReference<MediaTypeT>
        where MediaTypeT : MediaType
    {
        string CacheID { get; }
        MediaTypeT ContentType { get; }
    }

    public interface IStreamSource<MediaTypeT> : IContentReference<MediaTypeT>
        where MediaTypeT : MediaType
    {
        Task<Stream> GetStream(IProgress prog);
    }

    public static class IStreamSourceExt
    {
        public static Task<Stream> GetStream<MediaTypeT>(this IStreamSource<MediaTypeT> source)
            where MediaTypeT : MediaType
        {
            return source.GetStream(null);
        }

        public static async Task<MediaTypeT> Decode<MediaTypeT>(this IStreamSource<MediaTypeT> source, IDeserializer deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var stream = await source.GetStream(progs[0]);
            if (stream == null)
            {
                return default;
            }
            else
            {
                using (stream)
                {
                    return deserializer.Deserialize<MediaTypeT>(stream, progs[1]);
                }
            }
        }

        public static async Task<ResultT> Decode<MediaTypeT, ResultT>(this IStreamSource<MediaType> source, IDeserializer deserializer)
            where MediaTypeT : MediaType
        {
            var stream = await source.GetStream();
            if (stream == null)
            {
                return default;
            }
            else
            {
                using (stream)
                {
                    return deserializer.Deserialize<ResultT>(stream);
                }
            }
        }

        public static async Task<ResultT> Decode<MediaTypeT, ResultT>(this IStreamSource<MediaTypeT> source, IDeserializer<ResultT> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            var progs = prog.Split("Read", "Decode");
            var stream = await source.GetStream(progs[0]);
            if (stream == null)
            {
                return default;
            }
            else
            {
                using (stream)
                {
                    return deserializer.Deserialize(stream, progs[1]);
                }
            }
        }

        public static async Task<ResultT> Decode<MediaTypeT, ResultT>(this IStreamSource<MediaTypeT> source, IDeserializer<ResultT> deserializer)
            where MediaTypeT : MediaType
        {
            var stream = await source.GetStream();
            if (stream == null)
            {
                return default;
            }
            else
            {
                using (stream)
                {
                    return deserializer.Deserialize(stream);
                }
            }
        }

        public static Task Proxy<MediaTypeT>(this IStreamSource<MediaTypeT> source, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            response.ContentType = source.ContentType;
            return source.GetStream().Proxy(response);
        }

        public static Task Proxy<MediaTypeT>(this IStreamSource<MediaTypeT> source, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return source.Proxy(context.Response);
        }

        public static async Task<Stream> Cache<MediaTypeT>(this IStreamSource<MediaTypeT> source, ICacheLayer cache, IProgress prog)
            where MediaTypeT : MediaType
        {
            return cache.Cache(source, await source.GetStream(prog));
        }

        public static async Task<Stream> Cache<MediaTypeT>(this IStreamSource<MediaTypeT> source, ICacheLayer cache)
            where MediaTypeT : MediaType
        {
            return cache.Cache(source, await source.GetStream());
        }
    }
}
