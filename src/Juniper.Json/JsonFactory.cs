using System.IO;

using Json.Lite;

using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

namespace Juniper.Json
{
    public class JsonFactory : IFactory
    {
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
            using (var writer = new StreamWriter(stream))
            {
                var text = ToString(value);
                var length = System.Text.Encoding.UTF8.GetByteCount(text);
                writer.Write(new ProgressStream(stream, length, prog));
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Parse<T>(reader.ReadToEnd());
            }
        }
    }
}