using System.IO;
using System.Runtime.Serialization;
using Json.Lite;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;

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

        public void Serialize<T>(Stream stream, T value, IProgress prog = null)
        {
            prog?.Report(0);
            var writer = new StreamWriter(stream);
            var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented
            };
            var serializer = new JsonSerializer();
            serializer.Serialize(jsonWriter, value);
            jsonWriter.Flush();
            writer.Flush();
            stream.Flush();
            prog?.Report(1);
        }

        public T Deserialize<T>(Stream stream, IProgress prog = null)
        {
            prog?.Report(0);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var obj = serializer.Deserialize<T>(jsonReader);
            prog?.Report(1);
            return obj;
        }
    }
}