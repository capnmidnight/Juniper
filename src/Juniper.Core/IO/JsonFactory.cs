using System.IO;

using Json.Lite;

using Juniper.Progress;

namespace Juniper.IO
{
    public class JsonFactory : IFactory
    {
        public JsonFactory(MediaType contentType)
        {
            ContentType = contentType;
        }

        public JsonFactory()
            : this(MediaType.Application.Json)
        { }

        public MediaType ContentType
        {
            get;
        }

        public T Deserialize<T>(Stream stream, IProgress prog)
        {
            prog.Report(0);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var obj = serializer.Deserialize<T>(jsonReader);
            prog.Report(1);
            return obj;
        }

        public void Serialize<T>(Stream stream, T value, IProgress prog)
        {
            prog.Report(0);
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
            prog.Report(1);
        }
    }
}