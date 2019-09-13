using System.IO;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class SpecializedSerializer<T> : ISerializer<T>
    {
        private readonly ISerializer serializer;

        public SpecializedSerializer(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        public void Serialize(Stream stream, T value, IProgress prog)
        {
            serializer.Serialize(stream, value, prog);
        }
    }
}