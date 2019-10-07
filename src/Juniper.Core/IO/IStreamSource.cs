using System.IO;
using System.Net;
using System.Threading.Tasks;

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

        public static async Task<T> Decode<MediaTypeT, T>(this IStreamSource<MediaTypeT> source, IDeserializer<T> deserializer, IProgress prog)
            where MediaTypeT : MediaType
        {
            prog.Report(0);
            var progs = prog.Split("Read", "Decode");
            var stream = await source.GetStream(progs[0]);
            var value = stream.Decode(deserializer, progs[1]);
            prog.Report(1);
            return value;
        }

        public static Task<T> Decode<MediaTypeT, T>(this IStreamSource<MediaTypeT> source, IDeserializer<T> deserializer)
            where MediaTypeT : MediaType
        {
            return Decode(source, deserializer, null);
        }

        public static async Task Proxy<MediaTypeT>(this IStreamSource<MediaTypeT> source, HttpListenerResponse response)
            where MediaTypeT : MediaType
        {
            var stream = await source.GetStream();
            response.ContentType = source.ContentType;
            await stream.Proxy(response);
        }

        public static Task Proxy<MediaTypeT>(this IStreamSource<MediaTypeT> source, HttpListenerContext context)
            where MediaTypeT : MediaType
        {
            return source.Proxy(context.Response);
        }
    }
}
