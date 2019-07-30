using System.IO;

namespace Juniper.Serialization
{
    public class SpecializedSerializer<T> : ISerializer<T>
    {
        private readonly ISerializer serializer;

        public SpecializedSerializer(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public void Serialize(Stream stream, T value)
        {
            serializer.Serialize(stream, value);
        }
    }
}