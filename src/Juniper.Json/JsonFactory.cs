using System.IO;

using Json.Lite;

using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Json
{
    public abstract class AbstractJsonFactory
    {
        public MediaType ContentType { get; private set; }

        protected AbstractJsonFactory(MediaType contentType)
        {
            ContentType = contentType;
        }

        protected AbstractJsonFactory() : this(MediaType.Application.Json) { }

        protected static void InternalSerialize<T>(Stream stream, T value, IProgress prog)
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

        protected static T InternalDeserialize<T>(Stream stream, IProgress prog)
        {
            prog.Report(0);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            var obj = serializer.Deserialize<T>(jsonReader);
            prog.Report(1);
            return obj;
        }
    }

    public class JsonFactory : AbstractJsonFactory, IFactory
    {
        public JsonFactory(MediaType contentType)
            : base(contentType)
        { }

        public JsonFactory()
            : base()
        { }

        public T Deserialize<T>(Stream stream, IProgress prog)
        {
            return InternalDeserialize<T>(stream, prog);
        }

        public void Serialize<T>(Stream stream, T value, IProgress prog)
        {
            InternalSerialize(stream, value, prog);
        }
    }

    public class JsonFactory<T> : AbstractJsonFactory, IFactory<T>
    {
        public JsonFactory(MediaType contentType)
            : base(contentType)
        { }

        public JsonFactory()
            : base()
        { }

        public T Deserialize(Stream stream, IProgress prog)
        {
            return InternalDeserialize<T>(stream, prog);
        }

        public void Serialize(Stream stream, T value, IProgress prog)
        {
            InternalSerialize(stream, value, prog);
        }
    }
}