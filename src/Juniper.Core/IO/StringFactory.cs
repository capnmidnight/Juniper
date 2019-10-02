using System.IO;

using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public class StringFactory : IFactory<string>
    {
        public StringFactory(MediaType.Text contentType)
        {
            ContentType = contentType;
        }

        public StringFactory()
            : this(MediaType.Text.Plain)
        { }

        public MediaType.Text ContentType
        {
            get;
        }

        public string Deserialize(Stream stream, IProgress prog)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Serialize(Stream stream, string value, IProgress prog)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(value);
            }
        }
    }
}
