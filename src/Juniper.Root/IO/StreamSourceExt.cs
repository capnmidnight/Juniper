using System.IO;
using System.Net;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{

    public static class StreamSourceExt
    {
        public static async Task<ResultT> DecodeAsync<ResultT>(this StreamSource source, IDeserializer<ResultT> deserializer, IProgress prog = null)
        {
            prog.Report(0);
            var progs = prog.Split("Read", "Decode");
            var stream = await source
                .GetStreamAsync(progs[0])
                .ConfigureAwait(false);
            var value = deserializer.Deserialize(stream, progs[1]);
            prog.Report(1);
            return value;
        }

        public static Task<Stream> GetStreamAsync(this StreamSource source)
        {
            return source.GetStreamAsync(null);
        }

        public static async Task ProxyAsync(this StreamSource source, HttpListenerResponse response)
        {
            var stream = await source
                .GetStreamAsync()
                .ConfigureAwait(false);
            response.ContentType = source.ContentType;
            await stream
                .ProxyAsync(response)
                .ConfigureAwait(false);
        }

        public static Task ProxyAsync(this StreamSource source, HttpListenerContext context)
        {
            return source.ProxyAsync(context.Response);
        }
    }
}
