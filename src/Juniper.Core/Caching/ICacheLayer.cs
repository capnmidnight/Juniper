using System.IO;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Caching
{
    public interface ICacheLayer
    {
        bool CanCache { get; }

        bool IsCached(string fileDescriptor, MediaType contentType);

        Task<Stream> GetStream(string fileDescriptor, MediaType contentType, IProgress prog);

        Stream WrapStream(string fileDescriptor, MediaType contentType, Stream stream);

        Stream OpenWrite(string fileDescriptor, MediaType contentType);

        void Copy(FileInfo file, string fileDescriptor, MediaType contentType);
    }

    public static class ICacheLayerExt
    {
        public static Task<Stream> GetStream(this ICacheLayer layer, string fileDescriptor, MediaType contentType)
        {
            return layer.GetStream(fileDescriptor, contentType, null);
        }

        public static async Task<T> GetDecoded<T>(this ICacheLayer layer, string fileDescriptor, IDeserializer<T> decoder, IProgress prog)
        {
            var progs = prog.Split("Get", "Decode");
            using (var stream = await layer.GetStream(fileDescriptor, decoder.ReadContentType, progs[0]))
            {
                return decoder.Deserialize(stream, progs[1]);
            }
        }

        public static Task<T> GetDecoded<T>(this ICacheLayer layer, string fileDescriptor, IDeserializer<T> decoder)
        {
            return layer.GetDecoded(fileDescriptor, decoder, null);
        }

        public static void Save<T>(this ICacheLayer layer, string fileDescriptor, ISerializer<T> encoder, T value, IProgress prog)
        {
            using (var stream = layer.OpenWrite(fileDescriptor, encoder.WriteContentType))
            {
                encoder.Serialize(stream, value, prog);
                stream.Flush();
            }
        }

        public static void Save<T>(this ICacheLayer layer, string fileDescriptor, ISerializer<T> encoder, T value)
        {
            layer.Save(fileDescriptor, encoder, value, null);
        }

    }
}
