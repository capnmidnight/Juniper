using System;
using System.IO;

using Newtonsoft.Json;

namespace Juniper.IO
{

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

        public ResultT Deserialize(Stream stream)
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
                Formatting = Formatting.Indented
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