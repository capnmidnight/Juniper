using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Juniper.Compression.Tar
{
    public class TarArchive : IDisposable
    {
        internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private List<TarArchiveEntry> entries;

        private bool atEnd;

        private bool disposedValue = false;

        private long position;

        public TarArchive(Stream stream)
        {
            entries = ReadEntries(stream).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    entries = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IReadOnlyCollection<TarArchiveEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        public TarArchiveEntry GetEntry(string name)
        {
            foreach (var entry in Entries)
            {
                if (entry.FullName == name)
                {
                    return entry;
                }
            }

            return null;
        }

        private IEnumerable<TarArchiveEntry> ReadEntries(Stream stream)
        {
            var header = new byte[512];

            var zeroBlockCount = 0;
            while (zeroBlockCount < 2 && !atEnd)
            {
                var length = stream.Read(header, 0, header.Length);
                if (length < 512)
                {
                    throw new InvalidDataException($"Invalid header block size < 512");
                }

                position += length;

                if (IsAllZeros(header))
                {
                    ++zeroBlockCount;
                }
                else
                {
                    zeroBlockCount = 0;

                    var fileName = GetString(header, 0, 100);
                    ValidateChecksum(header, fileName);
                    var fileLength = ReadFileSize(header, fileName);

                    if (IsFileType(header))
                    {
                        var lastModifiedTime = ReadLastModifiedTime(header, fileName);
                        fileName = AddFilePrefix(header, fileName);
                        var entry = new TarArchiveEntry(stream, fileName, lastModifiedTime, fileLength);
                        position += fileLength;
                        position = SeekToNextEntry(header, stream, position, fileName);
                        yield return entry;
                    }
                }
            }

            atEnd = true;
        }

        private long SeekToNextEntry(byte[] header, Stream stream, long position, string fileName)
        {
            // The end of the file entry is aligned on 512 bytes
            var delta = (512 - (int)(position & 511)) % 512;
            if (delta > 0)
            {
                var read = stream.Read(header, 0, delta);
                if (read < delta)
                {
                    throw new InvalidDataException($"Invalid end of entry after file entry [{fileName}] ");
                }
            }
            return position + delta;
        }

        private static bool IsAllZeros(byte[] header)
        {
            for (var i = 0; i < header.Length; i++)
            {
                if (header[i] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFileType(byte[] header)
        {
            return header[156] == '0';
        }

        private static string AddFilePrefix(byte[] header, string fileName)
        {
            // Double check magic ustar to load prefix filename
            var ustar = GetString(header, 257, 8);
            // Check for ustar only
            if (ustar.Trim() == "ustar")
            {
                var prefixFileName = GetString(header, 345, 155);
                fileName = prefixFileName + fileName;
            }

            return fileName;
        }

        private static DateTime ReadLastModifiedTime(byte[] header, string fileName)
        {
            var unixTimeStamp = ReadOctal(header, 136, 12);
            if (!unixTimeStamp.HasValue)
            {
                throw new InvalidDataException($"Invalid timestamp for file entry [{fileName}] ");
            }
            var lastModifiedTime = Epoch.AddSeconds(unixTimeStamp.Value).ToLocalTime();
            return lastModifiedTime;
        }

        private static long ReadFileSize(byte[] header, string fileName)
        {
            var fileSizeRead = ReadOctal(header, 124, 12);
            if (!fileSizeRead.HasValue)
            {
                throw new InvalidDataException($"Invalid filesize for file entry [{fileName}] ");
            }

            var fileLength = fileSizeRead.Value;
            return fileLength;
        }

        private static void ValidateChecksum(byte[] header, string fileName)
        {
            // read checksum
            var checksum = ReadOctal(header, 148, 8);
            if (!checksum.HasValue)
            {
                throw new InvalidDataException($"Invalid checksum for file entry [{fileName}] ");
            }

            // verify checksum
            uint checksumVerif = 0;
            for (var i = 0; i < header.Length; i++)
            {
                var c = header[i];
                if (i >= 148 && i < (148 + 8))
                {
                    c = 32;
                }
                checksumVerif += c;
            }

            // Checksum is invalid, exit
            if (checksum.Value != checksumVerif)
            {
                throw new InvalidDataException($"Invalid checksum verification for file entry [{fileName}] ");
            }
        }

        /// <summary>
        /// Gets an ASCII string ending by a `\0`
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>A string</returns>
        private static string GetString(byte[] buffer, int index, int count)
        {
            var text = new StringBuilder();
            for (var i = index; i < index + count; i++)
            {
                if (buffer[i] == 0 || buffer[i] >= 127)
                {
                    break;
                }

                text.Append((char)buffer[i]);
            }
            return text.ToString();
        }

        /// <summary>
        /// Reads an octal number converted to integer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>An octal number converted to a long; otherwise <c>null</c> if the conversion failed</returns>
        private static long? ReadOctal(byte[] buffer, int index, int count)
        {
            long value = 0;
            for (var i = index; i < index + count; i++)
            {
                var c = buffer[i];
                if (c == 0)
                {
                    break;
                }
                if (c == ' ')
                {
                    continue;
                }
                if (c < '0' || c > '7')
                {
                    return null;
                }
                value = (value << 3) + (c - '0');
            }
            return value;
        }
    }
}
