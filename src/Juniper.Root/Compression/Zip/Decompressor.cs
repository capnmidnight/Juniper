using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using Juniper.Progress;

namespace Juniper.Compression.Zip
{
    /// <summary>
    /// Functions for dealing with Zip files.
    /// </summary>
    public static class Decompressor
    {
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
            return new ZipArchive(stream);
        }

        public static ZipArchive Open(string fileName)
        {
            return Open(new FileInfo(fileName.ValidateFileName()));
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

        public static void CopyFile(this ZipArchive zip, string entryPath, string copyToFileName, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            zip.CopyFile(entryPath, new FileInfo(copyToFileName.ValidateFileName()), prog);
        }

        public static void CopyFile(FileInfo file, string entryPath, Stream copyTo, IProgress prog = null)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (copyTo is null)
            {
                throw new ArgumentNullException(nameof(copyTo));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found", file.FullName);
            }

            using var zip = Open(file);
            zip.CopyFile(entryPath, copyTo, prog);
        }

        public static void CopyFile(string fileName, string entryPath, Stream copyTo, IProgress prog = null)
        {
            if (copyTo is null)
            {
                throw new ArgumentNullException(nameof(copyTo));
            }

            CopyFile(new FileInfo(fileName.ValidateFileName()), entryPath, copyTo, prog);
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

            var zip = Open(file);
            var entry = zip.GetEntry(entryPath);
            var stream = new ZipArchiveEntryStream(zip, entry);
            return new ProgressStream(stream, entry.Length, prog, true);
        }

        public static Stream GetFile(string fileName, string entryPath, IProgress prog = null)
        {
            return GetFile(new FileInfo(fileName.ValidateFileName()), entryPath, prog);
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

        public static IEnumerable<CompressedFileInfo> Entries(Stream zipStream, IProgress prog = null)
        {
            if (zipStream is null)
            {
                throw new ArgumentNullException(nameof(zipStream));
            }

            using var zip = new ZipArchive(zipStream);
            foreach (var entry in zip.Entries(prog))
            {
                yield return entry;
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo zipFile, IProgress prog = null)
        {
            if (zipFile is null)
            {
                throw new ArgumentNullException(nameof(zipFile));
            }

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("File not found", zipFile.FullName);
            }

            using var stream = zipFile.OpenRead();
            foreach (var entry in Entries(stream, prog))
            {
                yield return entry;
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(string zipFileName, IProgress prog = null)
        {
            return Entries(new FileInfo(zipFileName.ValidateFileName()), prog);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog = null)
        {
            if (zip is null)
            {
                throw new ArgumentNullException(nameof(zip));
            }

            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            var noPrefix = string.IsNullOrEmpty(entryPrefix);
            var i = 0;
            foreach (var entry in zip.Entries)
            {
                prog.Report(i++, zip.Entries.Count);
                try
                {
                    var fileName = entry.FullName;
                    if (noPrefix || fileName.StartsWith(entryPrefix, StringComparison.InvariantCulture))
                    {
                        fileName = fileName.Remove(0, entryPrefix.Length);
                        var outputPath = Path.Combine(outputDirectory.FullName, fileName);
                        var outputFile = new FileInfo(outputPath);
                        var outputFileDirectory = outputFile.Directory;

                        if (overwrite || !outputFile.Exists)
                        {
                            if (!outputFileDirectory.Exists)
                            {
                                outputFileDirectory.Create();
                            }

                            using var outputStream = outputFile.Create();
                            using var inputStream = entry.Open();
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
                catch { }
            }

            prog.Report(i, zip.Entries.Count);
        }

        public static void Decompress(Stream stream, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            using var zip = new ZipArchive(stream);
            zip.Decompress(outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog = null)
        {
            if (zipFile is null)
            {
                throw new ArgumentNullException(nameof(zipFile));
            }

            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("File not found", zipFile.FullName);
            }

            using var stream = zipFile.OpenRead();
            Decompress(stream, outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            if (zipFile is null)
            {
                throw new ArgumentNullException(nameof(zipFile));
            }

            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("File not found", zipFile.FullName);
            }

            using var stream = zipFile.OpenRead();
            Decompress(stream, outputDirectory, null, true, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog = null)
        {
            if (outputDirectory is null)
            {
                throw new ArgumentNullException(nameof(outputDirectory));
            }

            Decompress(new FileInfo(zipFileName.ValidateFileName()), outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, string entryPrefix, bool overwrite, IProgress prog = null)
        {
            if (outputDirectoryName is null)
            {
                throw new ArgumentNullException(nameof(outputDirectoryName));
            }

            Decompress(zipFileName.ValidateFileName(), new DirectoryInfo(outputDirectoryName), entryPrefix, overwrite, prog);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, IProgress prog = null)
        {
            if (zipFileName is null)
            {
                throw new ArgumentNullException(nameof(zipFileName));
            }

            if (outputDirectoryName is null)
            {
                throw new ArgumentNullException(nameof(outputDirectoryName));
            }

            Decompress(zipFileName, outputDirectoryName, null, true, prog);
        }
    }
}