using System;
using System.IO;

using Juniper.IO;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public class ShaderDeserializer : IDeserializer<ShaderData>, IContentHandler<MediaType.Text>
    {
        public MediaType.Text ContentType => MediaType.Text.Plain;

        public ShaderData Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var mem = new MemoryStream();
            stream.CopyTo(mem);
            return new ShaderData(mem.ToArray());
        }
    }
}
