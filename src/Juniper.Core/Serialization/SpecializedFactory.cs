using System.IO;
using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class SpecializedFactory<T> : IFactory<T>
    {
        private readonly IFactory factory;

        public SpecializedFactory(IFactory factory, MediaType contentType)
        {
            this.factory = factory;
            ReadContentType = contentType;
        }

        public void Serialize(Stream stream, T value, IProgress prog)
        {
            factory.Serialize(stream, value, prog);
        }

        public T Deserialize(Stream stream, IProgress prog)
        {
            return factory.Deserialize<T>(stream, prog);
        }

        public MediaType ReadContentType
        {
            get;
            private set;
        }

        public MediaType WriteContentType
        {
            get
            {
                return ReadContentType;
            }
        }
    }
}