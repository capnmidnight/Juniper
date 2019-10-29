using System.IO;

using Json.Lite;

using Juniper.Progress;

namespace Juniper.IO
{

    public class JsonFactory<T> : JsonFactory<T, MediaType.Application>, IJsonDecoder<T>
    {
        public JsonFactory() : base(MediaType.Application.Json)
        { }
    }

    public class JsonFactory<ResultT, MediaTypeT> : IFactory<ResultT, MediaTypeT>
        where MediaTypeT : MediaType
    {
        public JsonFactory(MediaTypeT contentType)
        {
            ContentType = contentType;
        }

        public MediaTypeT ContentType
        {
            get;
        }

        public ResultT Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            ResultT value = default;
            if (stream != null)
            {
                using (stream)
                {
                    var reader = new StreamReader(stream);
                    var jsonReader = new JsonTextReader(reader);
                    var serializer = new JsonSerializer();
                    value = serializer.Deserialize<ResultT>(jsonReader);
                }
            }
            prog.Report(1);
            return value;
        }

        public void Serialize(Stream stream, ResultT value, IProgress prog)
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