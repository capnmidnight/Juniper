using System;
using System.IO;
using System.Net;
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

        public void Serialize(HttpListenerResponse response, ResultT value)
        {
            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.ContentType = ContentType;
            response.ContentLength64 = Serialize(response.OutputStream, value);
        }


        public void Serialize(HttpWebRequest request, ResultT value)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var stream = request.GetRequestStream();
            request.ContentType = ContentType;
            request.ContentLength = Serialize(stream, value);
        }
    }
}