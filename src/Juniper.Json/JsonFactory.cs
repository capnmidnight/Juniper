using System.IO;

using Json.Lite;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public class JsonFactory : ITextDecoder
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

    public class JsonFactory<T> : ITextDecoder<T>
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

        public T Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            T value = default;
            if (stream != null)
            {
                using (stream)
                {
                    var reader = new StreamReader(stream);
                    var jsonReader = new JsonTextReader(reader);
                    var serializer = new JsonSerializer();
                    value = serializer.Deserialize<T>(jsonReader);
                }
            }
            prog.Report(1);
            return value;
        }

        public void Serialize(Stream stream, T value, IProgress prog)
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