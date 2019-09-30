using System.IO;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.Serialization
{
    public class StreamStringDecoder : IDeserializer<string>
    {
        public StreamStringDecoder(MediaType contentType)
        {
            ReadContentType = contentType;
        }

        public StreamStringDecoder()
            : this(MediaType.Text.Plain)
        { }

        public MediaType ReadContentType
        {
            get;
            private set;
        }

        public string Deserialize(Stream stream, IProgress prog)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
