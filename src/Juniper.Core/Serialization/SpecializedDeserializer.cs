using System.IO;

namespace Juniper.Serialization
{
    public class SpecializedDeserializer<T> : IDeserializer<T>
    {
        private readonly IDeserializer deserializer;

        public SpecializedDeserializer(IDeserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        public T Deserialize(Stream stream)
        {
            return deserializer.Deserialize<T>(stream);
        }
    }
}