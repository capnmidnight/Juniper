using System.IO;

using Juniper.IO;

using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public class MtlDeserializer : IDeserializer<MtlFile>, IContentHandler<MediaType.Text>
    {
        public MediaType.Text ContentType => MediaType.Text.Plain;

        public MtlFile Deserialize(Stream stream)
        {
            var parser = new MtlParser();
            return parser.Parse(stream);
        }
    }
}
