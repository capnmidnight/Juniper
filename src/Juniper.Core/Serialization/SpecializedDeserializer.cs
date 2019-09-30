using System;
using System.IO;
using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class SpecializedDeserializer<T> : IDeserializer<T>
    {
        private readonly IDeserializer deserializer;

        public SpecializedDeserializer(IDeserializer deserializer, MediaType contentType)
        {
            this.deserializer = deserializer;
            ReadContentType = contentType;
        }

        public T Deserialize(Stream stream, IProgress prog)
        {
            return deserializer.Deserialize<T>(stream, prog);
        }

        public MediaType ReadContentType
        {
            get;
            private set;
        }
    }
}