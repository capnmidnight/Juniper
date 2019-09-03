using System.IO;
using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class SpecializedFactory<T> : IFactory<T>
    {
        private readonly IFactory factory;

        public SpecializedFactory(IFactory factory)
        {
            this.factory = factory;
        }

        public void Serialize(Stream stream, T value, IProgress prog = null)
        {
            factory.Serialize(stream, value, prog);
        }

        public T Deserialize(Stream stream, IProgress prog = null)
        {
            return factory.Deserialize<T>(stream, prog);
        }

        public MediaType ContentType { get { return factory.ContentType; } }
    }
}