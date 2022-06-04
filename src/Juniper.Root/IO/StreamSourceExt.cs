using Juniper.Progress;

namespace Juniper.IO
{

    public static class StreamSourceExt
    {
        public static async Task<ResultT> DecodeAsync<ResultT, M>(this AbstractStreamSource source, IDeserializer<ResultT, M> deserializer, IProgress prog = null)
            where M : MediaType
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
            using var stream = await source
                .GetStreamAsync(prog)
                .ConfigureAwait(false);
            var value = deserializer.Deserialize(stream);
            prog.Report(1);
            return value;
        }

        public static Task<Stream> GetStreamAsync(this AbstractStreamSource source)
        {
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            return source.GetStreamAsync(null);
        }
    }
}
