using System;
using System.IO;

namespace Juniper.Compression.Tar
{

    /// <summary>
    /// An Tar entry stream for a file entry from a tar stream.
    /// </summary>
    /// <seealso cref="Stream" />
    public class TarArchiveEntry
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

        public long Length
        {
            get
            {
                return copy.Length;
            }
        }

        public Stream Open()
        {
            copy.Position = 0;
            return copy;
        }
    }
}
