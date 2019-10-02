using System.IO;

namespace Juniper.IO
{
    /// <summary>
    /// Wraps a non-seekable stream (like <see cref="System.Net.ConnectStream"/>) for use
    /// with poorly-designed libraries that expect seekable streams.
    /// </summary>
    public class ErsatzSeekableStream : MemoryStream
    {
        public ErsatzSeekableStream(Stream stream)
        {
            using (stream)
            {
                stream.CopyTo(this);
            }
            Position = 0;
        }
    }
}