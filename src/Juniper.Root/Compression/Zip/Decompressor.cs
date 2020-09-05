using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Compression.Zip
{
    /// <summary>
    /// Functions for dealing with Zip files.
    /// </summary>
    public static class Decompressor
    {
        private static ZipArchive Open(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new ZipArchive(stream);
        }

        public static ZipArchive Open(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            var stream = file.OpenRead();
            return Open(stream);
        }

        public static ZipArchive Open(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return Open(new FileInfo(fileName));
        }

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="fileName">The zip file in which to find the file.</param>
        /// <param name="entryPath">The file to find in the zip file.</param>
        /// <param name="copyTo">A stream to which to copy the file, before closing the zipFile.</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        public static void CopyFile(this ZipArchive zip, string entryPath, Stream copyTo, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            if (copyTo is null)
            {
                throw new ArgumentNullException(nameof(copyTo));
            }

            var entry = zip.GetEntry(entryPath);
            if (entry is null)
            {
                throw new FileNotFoundException($"Could not find file {entryPath} in the zip file.");
            }

            using var copyFrom = entry.Open();
            using var progStream = new ProgressStream(copyFrom, entry.Length, prog, false);
            progStream.CopyTo(copyTo);
        }

        public static void CopyFile(this ZipArchive zip, string entryPath, FileInfo copyToFile, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            if (copyToFile is null)
            {
                throw new ArgumentNullException(nameof(copyToFile));
            }

            copyToFile.Directory.Create();
            using var copyTo = copyToFile.Create();
            zip.CopyFile(entryPath, copyTo, prog);
        }

        public static void CopyFile(this ZipArchive copyFromZip, string entryPath, string copyToFileName, IProgress prog = null)
        {
            if (copyFromZip is null)
            {
                throw new ArgumentNullException(nameof(copyFromZip));
            }

            if (copyToFileName is null)
            {
                throw new ArgumentNullException(nameof(copyToFileName));
            }

            if (copyToFileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(copyToFileName));
            }


            copyFromZip.CopyFile(entryPath, new FileInfo(copyToFileName), prog);
        }

        public static void CopyFile(Stream copyFromStream, string entryPath, Stream copyToStream, IProgress prog = null)
        {
            if (copyFromStream is null)
            {
                throw new ArgumentNullException(nameof(copyFromStream));
            }

            if (copyToStream is null)
            {
                throw new ArgumentNullException(nameof(copyToStream));
            }

            using var zip = Open(copyFromStream);
            zip.CopyFile(entryPath, copyToStream, prog);
        }

        public static void CopyFile(FileInfo copyFromFile, string entryPath, Stream copyToStream, IProgress prog = null)
        {
            if (copyFromFile is null)
            {
                throw new ArgumentNullException(nameof(copyFromFile));
            }

            if (!copyFromFile.Exists)
            {
                throw new FileNotFoundException("File not found", copyFromFile.FullName);
            }

            using var stream = copyFromFile.OpenRead();
            CopyFile(stream, entryPath, copyToStream, prog);
        }

        public static void CopyFile(string copyFromFileName, string entryPath, Stream copyToStream, IProgress prog = null)
        {
            if (copyToStream is null)
            {
                throw new ArgumentNullException(nameof(copyToStream));
            }

            if (copyFromFileName is null)
            {
                throw new ArgumentNullException(nameof(copyFromFileName));
            }

            if (copyFromFileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(copyFromFileName));
            }

            CopyFile(new FileInfo(copyFromFileName), entryPath, copyToStream, prog);
        }

        public static Stream GetFile(this ZipArchive zip, string entryPath, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            var entry = zip.GetEntry(entryPath);
            return new ProgressStream(entry.Open(), entry.Length, prog, true);
        }

        public static Stream GetFile(Stream zipStream, string entryPath, IProgress prog = null)
        {
            if (zipStream is null)
            {
                throw new ArgumentNullException(nameof(zipStream));
            }

            var zip = Open(zipStream);
            var entry = zip.GetEntry(entryPath);
            var stream = new ZipArchiveEntryStream(zip, entry);
            return new ProgressStream(stream, entry.Length, prog, true);
        }

        public static Stream GetFile(FileInfo file, string entryPath, IProgress prog = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found", file.FullName);
            }

            var stream = file.OpenRead();
            return GetFile(stream, entryPath, prog);
        }

        public static Stream GetFile(string fileName, string entryPath, IProgress prog = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return GetFile(new FileInfo(fileName), entryPath, prog);
        }

        /// <summary>
        /// Enumerates all of the entires in a single Zip archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip">A zipfile stream</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        /// <returns>A lazy collection of <typeparamref name="ZipArchiveEntry"/> objects.</returns>
        public static IEnumerable<CompressedFileInfo> Entries(this ZipArchive zip, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            var i = 0;
            foreach (var entry in zip.Entries)
            {
                prog.Report(i++, zip.Entries.Count);
                yield return new CompressedFileInfo(entry);
            }

            prog.Report(i, zip.Entries.Count);
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream stream, IProgress prog = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var zip = Open(stream);
            foreach (var entry in zip.Entries(prog))
            {
                yield return entry;
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo file, IProgress prog = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found", file.FullName);
            }

            var stream = file.OpenRead();
            return Entries(stream, prog);
        }

        public static IEnumerable<CompressedFileInfo> Entries(string fileName, IProgress prog = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return Entries(new FileInfo(fileName), prog);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, string entryPrefix = null, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            if (outputDirectory.IsJunction())
            {
                return;
            }

            var noPrefix = string.IsNullOrEmpty(entryPrefix);
            var i = 0;
            foreach (var entry in zip.Entries)
            {
                prog.Report(i++, zip.Entries.Count);
                var fileName = entry.FullName;
                if (noPrefix || fileName.StartsWith(entryPrefix, StringComparison.InvariantCulture))
                {
                    if (!noPrefix)
                    {
                        fileName = fileName.Remove(0, entryPrefix.Length);
                    }
                    var outputPath = Path.Combine(outputDirectory.FullName, fileName);
                    var outputFile = new FileInfo(outputPath);
                    var outputFileDirectory = outputFile.Directory;

                    if (!outputFileDirectory.Exists)
                    {
                        outputFileDirectory.Create();
                    }

                    try
                    {
                        using var outputStream = outputFile.Create();
                        using var inputStream = entry.Open();
                        inputStream.CopyTo(outputStream);
                        outputStream.Flush();
                    }
                    catch { }
                }
            }

            prog.Report(i, zip.Entries.Count);
        }

        public static void Decompress(Stream stream, DirectoryInfo outputDirectory, string entryPrefix = null, IProgress prog = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            using var zip = Open(stream);
            zip.Decompress(outputDirectory, entryPrefix, prog);
        }

        public static void Decompress(Stream stream, string outputDirectoryName, string entryPrefix = null, IProgress prog = null)
        {
            if (outputDirectoryName is null)
            {
                throw new ArgumentNullException(nameof(outputDirectoryName));
            }

            Decompress(stream, new DirectoryInfo(outputDirectoryName), entryPrefix, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, string entryPrefix = null, IProgress prog = null)
        {
            if (zipFile is null)
            {
                throw new ArgumentNullException(nameof(zipFile));
            }

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException($"File not found: {zipFile.FullName}", zipFile.FullName);
            }

            var stream = zipFile.OpenRead();
            Decompress(stream, outputDirectory, entryPrefix, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, string entryPrefix = null, IProgress prog = null)
        {
            if (outputDirectoryName is null)
            {
                throw new ArgumentNullException(nameof(outputDirectoryName));
            }

            Decompress(zipFile, new DirectoryInfo(outputDirectoryName), entryPrefix, prog);
        }

        public static void Decompress(string fileName, DirectoryInfo outputDirectory, string entryPrefix = null, IProgress prog = null)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            Decompress(new FileInfo(fileName), outputDirectory, entryPrefix, prog);
        }

        public static void Decompress(string fileName, string outputDirectoryName, string entryPrefix = null, IProgress prog = null)
        {
            if (outputDirectoryName is null)
            {
                throw new ArgumentNullException(nameof(outputDirectoryName));
            }

            Decompress(fileName, new DirectoryInfo(outputDirectoryName), entryPrefix, prog);
        }
    }
}