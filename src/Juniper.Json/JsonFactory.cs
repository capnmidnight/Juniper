using System.IO;

using Json.Lite;
using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.Json
{
    public class JsonFactory : IFactory
    {
        public MediaType ContentType { get; private set; }

        public JsonFactory(MediaType contentType)
        {
            ContentType = contentType;
        }

        public JsonFactory() : this(MediaType.Application.Json) { }

        public string ToString<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Parse<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Serialize<T>(Stream stream, T value, IProgress prog = null)
        {
            var text = ToString(value);
            var length = System.Text.Encoding.UTF8.GetByteCount(text);
            var progress = new ProgressStream(stream, length, prog);
            var writer = new StreamWriter(progress);
            writer.Write(text);
        }

        public T Deserialize<T>(Stream stream)
        {
            var reader = new StreamReader(stream);
            return Parse<T>(reader.ReadToEnd());
        }
    }
}