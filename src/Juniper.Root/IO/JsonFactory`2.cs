using System.IO;

using Juniper.Progress;

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

        public ResultT Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            ResultT value = default;
            if (stream is object)
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

        public void Serialize(Stream stream, ResultT value, IProgress prog = null)
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