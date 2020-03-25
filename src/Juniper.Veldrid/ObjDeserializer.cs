using System.IO;
using Juniper.IO;

using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public class ObjDeserializer : IDeserializer<ObjFile>, IContentHandler<MediaType.Text>
    {
        public MediaType.Text ContentType => MediaType.Text.Plain;

        public ObjFile Deserialize(Stream stream)
        {
            var parser = new ObjParser();
            return parser.Parse(stream);
        }
    }
}
