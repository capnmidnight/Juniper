using Newtonsoft.Json;

namespace Juniper.IO
{

    public class JsonFactory<ResultT, MediaTypeT> : IFactory<ResultT, MediaTypeT>
        where MediaTypeT : MediaType
    {
        public Formatting Formatting { get; set; } = Formatting.Indented;

        public JsonFactory(MediaTypeT contentType)
        {
            InputContentType = contentType;
        }

        public MediaTypeT InputContentType
        {
            get;
        }

        public MediaTypeT OutputContentType => InputContentType;

        public ResultT? Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = new JsonSerializer();
            return serializer.Deserialize<ResultT>(jsonReader);
        }

        public long Serialize(Stream stream, ResultT value)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var writer = new StreamWriter(stream);
            using var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting
            };

            var serializer = new JsonSerializer();
            serializer.Serialize(jsonWriter, value);
            jsonWriter.Flush();
            writer.Flush();
            stream.Flush();
            return stream.Length;
        }
    }
}