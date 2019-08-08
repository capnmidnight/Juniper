using System.IO;

using Json.Lite;

using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.Json
{
    public class JsonFactory : IFactory
    {
#pragma warning disable CA1822 // Mark members as static

        public string ToString<T>(T value)
#pragma warning restore CA1822 // Mark members as static
        {
            return JsonConvert.SerializeObject(value);
        }

#pragma warning disable CA1822 // Mark members as static

        public T Parse<T>(string text)
#pragma warning restore CA1822 // Mark members as static
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Serialize<T>(Stream stream, T value, IProgress prog = null)
        {
            var text = ToString(value);
            var length = System.Text.Encoding.UTF8.GetByteCount(text);
            using (var progress = new ProgressStream(stream, length, prog))
            using (var writer = new StreamWriter(progress))
            {
                writer.Write(text);
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Parse<T>(reader.ReadToEnd());
            }
        }
    }
}