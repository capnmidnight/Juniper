using System.IO;

namespace Juniper.IO
{
    /// <summary>
    /// Wraps a non-seekable stream (like <see cref="System.Net.ConnectStream"/>) for use
    /// with poorly-designed libraries that expect seekable streams.
    /// </summary>
    public class ErsatzSeekableStream : MemoryStream
    {
        private readonly Stream stream;

        public ErsatzSeekableStream(Stream stream)
        {
            this.stream = stream ?? throw new System.ArgumentNullException(nameof(stream));
            stream.CopyTo(this);
            Position = 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                stream.Dispose();
            }
        }
    }
}