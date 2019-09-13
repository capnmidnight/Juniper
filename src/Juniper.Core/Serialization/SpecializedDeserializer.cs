using System;
using System.IO;
using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class SpecializedDeserializer<T> : IDeserializer<T>
    {
        private readonly IDeserializer deserializer;

        public SpecializedDeserializer(IDeserializer deserializer)
        {
            this.deserializer = deserializer;
        }

        public T Deserialize(Stream stream, IProgress prog)
        {
            return deserializer.Deserialize<T>(stream, prog);
        }

        public MediaType ContentType { get { return deserializer.ContentType; } }
    }
}