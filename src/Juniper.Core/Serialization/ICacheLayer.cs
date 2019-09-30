using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.HTTP.REST;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public interface ICacheLayer
    {
        bool IsCached(AbstractRequest request);

        Task<Stream> GetStream(AbstractRequest request, IProgress prog);

        Stream WrapStream(AbstractRequest request, Stream stream);
    }

    public static class ICacheLayerExt
    {
        public static Task<Stream> GetStream(this ICacheLayer source, AbstractRequest request)
        {
            return source.GetStream(request, null);
        }

        public static async Task<T> GetDecoded<T>(this ICacheLayer source, AbstractRequest request, IDeserializer<T> decoder, IProgress prog)
        {
            var split = prog.Split("Get", "Decode");
            using (var stream = await source.GetStream(request, split[0]))
            {
                return decoder.Deserialize(stream, split[1]);
            }
        }

        public static Task<T> GetDecoded<T>(this ICacheLayer source, AbstractRequest request, IDeserializer<T> decoder)
        {
            return source.GetDecoded(request, decoder, null);
        }

        public static async Task Proxy(this ICacheLayer source, HttpListenerResponse outResponse, AbstractRequest request)
        {
            var inStream = await source.GetStream(request, null);
            if (inStream == null)
            {
                outResponse.SetStatus(HttpStatusCode.NotFound);
            }
            else
            {
                using (inStream)
                {
                    outResponse.SetStatus(HttpStatusCode.OK);
                    if (inStream.Length >= 0)
                    {
                        outResponse.ContentType = request.ContentType;
                        outResponse.ContentLength64 = inStream.Length;
                    }
                    inStream.CopyTo(outResponse.OutputStream);
                }
            }
        }
    }
}
