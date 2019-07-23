using System.IO;

namespace Juniper.Serialization
{
    public interface IDeserializer
    {
        bool TryDeserialize<T>(string text, out T value);

        T Deserialize<T>(string text);
    }

    public static class IDeserializerExt
    {
        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return deserializer.Deserialize<T>(reader.ReadToEnd());
            }
        }
    }
}