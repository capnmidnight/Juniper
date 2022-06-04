using Juniper.Progress;

using System.Text;

namespace Juniper.Compression.Tar
{
    public sealed class TarArchive : IDisposable
    {
        #region Tar file parsing functions

        private static long SeekToNextEntry(byte[] header, Stream stream, long position, string fileName)
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
            var ustar = GetString(header, 257, Units.Bits.PER_BYTE);
            // Check for ustar only
            if (ustar.Trim() == "ustar")
            {
                var prefixFileName = GetString(header, 345, 155);
                fileName = prefixFileName + fileName;
            }

            return fileName;
        }

        private static DateTimeOffset ReadLastModifiedTime(byte[] header, string fileName)
        {
            var unixTimeStamp = ReadOctal(header, 136, 12);
            if (!unixTimeStamp.HasValue)
            {
                throw new InvalidDataException($"Invalid timestamp for file entry [{fileName}] ");
            }

            return DateTimeExt.UnixTimestampToDateTime(unixTimeStamp.Value);
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
            var checksum = ReadOctal(header, 148, Units.Bits.PER_BYTE);
            if (!checksum.HasValue)
            {
                throw new InvalidDataException($"Invalid checksum for file entry [{fileName}] ");
            }

            // verify checksum
            uint checksumVerif = 0;
            for (var i = 0; i < header.Length; i++)
            {
                var c = header[i];
                if (i >= 148 && i < (148 + Units.Bits.PER_BYTE))
                {
                    c = Units.Bits.PER_INT;
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
        #endregion Tar file parsing functions

        private readonly List<TarArchiveEntry> entries;

        private bool disposedValue;

        public TarArchive(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            entries = new List<TarArchiveEntry>();

            var header = new byte[512];

            var zeroBlockCount = 0;
            var position = 0L;
            while (zeroBlockCount < 2)
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
                        var entry = new TarArchiveEntry(stream, fileName, lastModifiedTime.DateTime.ToLocalTime(), fileLength);
                        position += fileLength;
                        position = SeekToNextEntry(header, stream, position, fileName);
                        entries.Add(entry);
                    }
                }
            }
        }

        public IReadOnlyCollection<TarArchiveEntry> Entries => entries;

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

        public void Decompress(DirectoryInfo outputDirectory, bool overwrite, IProgress prog = null)
        {
            Decompress(outputDirectory, overwrite, null, prog);
        }

        public void Decompress(DirectoryInfo outputDirectory, bool overwrite = true, string entryPrefix = null, IProgress prog = null)
        {
            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            entryPrefix ??= string.Empty;

            for (var i = 0; i < entries.Count; ++i)
            {
                prog.Report(i, entries.Count);
                try
                {
                    var entry = entries[i];
                    var fileName = entry.FullName;
                    if (fileName.StartsWith(entryPrefix, StringComparison.InvariantCulture))
                    {
                        if (entryPrefix != null)
                        {
                            fileName = fileName.Remove(0, entryPrefix.Length);
                        }

                        var outputPath = Path.Combine(outputDirectory.FullName, fileName);
                        var outputFile = new FileInfo(outputPath);
                        var outputFileDirectory = outputFile.Directory;

                        if (overwrite || !outputFile.Exists)
                        {
                            outputFileDirectory.Create();
                            using var outputStream = outputFile.Create();
                            using var inputStream = entry.Open();
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
                catch { }
                prog.Report(i + 1, entries.Count);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var entry in entries)
                    {
                        entry.Dispose();
                    }

                    entries.Clear();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
