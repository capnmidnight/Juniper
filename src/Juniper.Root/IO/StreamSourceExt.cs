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
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            if (deserializer is null)
            {
                throw new System.ArgumentNullException(nameof(deserializer));
            }

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
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            return source.GetStreamAsync(null);
        }
    }
}
