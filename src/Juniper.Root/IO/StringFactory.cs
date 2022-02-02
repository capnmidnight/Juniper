using System;
using System.IO;
using System.Text;

namespace Juniper.IO
{
    public class StringFactory : IFactory<string, MediaType.Text>
    {
        public StringFactory(MediaType.Text contentType)
        {
            InputContentType = contentType;
        }

        public StringFactory()
            : this(MediaType.Text.Plain)
        { }

        public MediaType.Text InputContentType { get; }

        public MediaType.Text OutputContentType => InputContentType;

        public string Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public long Serialize(Stream stream, string value)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
            return bytes.Length;
        }
    }
}
