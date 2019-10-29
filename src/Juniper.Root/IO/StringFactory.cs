using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{
    public class StringFactory : IFactory<string, MediaType.Text>
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
            prog.Report(0);
            string value = null;
            if (stream != null)
            {
                using (stream)
                {
                    var reader = new StreamReader(stream);
                    value = reader.ReadToEnd();
                }
            }
            prog.Report(1);
            return value;
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
