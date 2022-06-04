namespace Juniper.Compression.Tar
{
    /// <summary>
    /// An Tar entry stream for a file entry from a tar stream.
    /// </summary>
    public sealed class TarArchiveEntry :
        IDisposable
    {
        private readonly MemoryStream copy;

        /// <summary>
        /// Initializes a new instance of the <see cref="TarArchiveEntry"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="lastModifiedTime">The timestamp on which the file was last modified.</param>
        /// <param name="length">The length.</param>
        ///
        /// <exception cref="ArgumentNullException"></exception>
        public TarArchiveEntry(Stream stream, string fileName, DateTime lastModifiedTime, long length)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            FullName = fileName;
            LastModifiedTime = lastModifiedTime;
            copy = new MemoryStream((int)length);
            var block = new byte[512];
            int read;

            while ((read = stream.Read(block, 0, Math.Min((int)length, block.Length))) > 0)
            {
                copy.Write(block, 0, read);
                length -= read;
            }
        }

        /// <summary>
        /// Gets the name of the file entry.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets the timestamp of the file entry.
        /// </summary>
        public DateTime LastModifiedTime { get; }

        public long Length => copy.Length;

        public Stream Open()
        {
            copy.Position = 0;
            return copy;
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    copy.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
