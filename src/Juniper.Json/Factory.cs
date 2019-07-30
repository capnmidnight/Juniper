using System.IO;

using Json.Lite;

using Juniper.Serialization;

namespace Juniper.Json
{
    public class Factory : IFactory
    {
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Serialize<T>(Stream stream, T value)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(Serialize(value));
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Deserialize<T>(reader.ReadToEnd());
            }
        }
    }


    public class Factory<T> : IFactory<T>
    {
        public string Serialize(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public T Deserialize(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Serialize(Stream stream, T value)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(Serialize(value));
            }
        }

        public T Deserialize(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Deserialize(reader.ReadToEnd());
            }
        }
    }
}