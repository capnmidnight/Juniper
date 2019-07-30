using System.IO;

namespace Juniper.Serialization
{
    public class SpecializedFactory<T> : IFactory<T>
    {
        private readonly IFactory factory;

        public SpecializedFactory(IFactory factory)
        {
            this.factory = factory;
        }

        public void Serialize(Stream stream, T value)
        {
            factory.Serialize(stream, value);
        }

        public T Deserialize(Stream stream)
        {
            return factory.Deserialize<T>(stream);
        }
    }
}