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
            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }
            else
            {
                return new ZipArchive(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
            }
        }

        public static ZipArchive Open(string fileName)
        {
            return Open(new FileInfo(fileName));
        }

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="fileName">The zip file in which to find the file.</param>
        /// <param name="entryPath">The file to find in the zip file.</param>
        /// <param name="copyTo">A stream to which to copy the file, before closing the zipFile.</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        public static void CopyFile(this ZipArchive zip, string entryPath, Stream copyTo, IProgress prog)
        {
            var entry = zip.GetEntry(entryPath);
            if (entry == null)
            {
                throw new FileNotFoundException($"Could not find file {entryPath} in the zip file.");
            }

            using var fileStream = entry.Open();
            var progStream = new ProgressStream(fileStream, entry.Length, prog);
            progStream.CopyTo(copyTo);
        }

        public static void CopyFile(this ZipArchive zip, string entryPath, Stream copyTo)
        {
            zip.CopyFile(entryPath, copyTo, null);
        }

        public static void CopyFile(this ZipArchive zip, string entryPath, FileInfo copyToFile, IProgress prog)
        {
            copyToFile.Directory.Create();
            using var copyTo = copyToFile.Create();
            zip.CopyFile(entryPath, copyTo, prog);
        }

        public static void CopyFile(this ZipArchive zip, string entryPath, FileInfo copyToFile)
        {
            zip.CopyFile(entryPath, copyToFile, null);
        }

        public static void CopyFile(this ZipArchive zip, string entryPath, string copyToFileName, IProgress prog)
        {
            zip.CopyFile(entryPath, new FileInfo(copyToFileName), prog);
        }

        public static void CopyFile(this ZipArchive zip, string entryPath, string copyToFileName)
        {
            zip.CopyFile(entryPath, copyToFileName, null);
        }

        public static void CopyFile(FileInfo file, string entryPath, Stream copyTo, IProgress prog)
        {
            using var zip = Open(file);
            zip.CopyFile(entryPath, copyTo, prog);
        }

        public static void CopyFile(FileInfo file, string entryPath, Stream copyTo)
        {
            CopyFile(file, entryPath, copyTo, null);
        }

        public static void CopyFile(string fileName, string entryPath, Stream copyTo, IProgress prog)
        {
            CopyFile(new FileInfo(fileName), entryPath, copyTo, prog);
        }

        public static void CopyFile(string fileName, string entryPath, Stream copyTo)
        {
            CopyFile(fileName, entryPath, copyTo, null);
        }

        public static Stream GetFile(this ZipArchive zip, string entryPath, IProgress prog)
        {
            var entry = zip.GetEntry(entryPath);
            var stream = entry.Open();
            if (prog != null)
            {
                stream = new ProgressStream(stream, entry.Length, prog);
            }

            return stream;
        }

        public static Stream GetFile(this ZipArchive zip, string entryPath)
        {
            return zip.GetFile(entryPath, null);
        }

        public static Stream GetFile(FileInfo file, string entryPath, IProgress prog)
        {
            var zip = Open(file);
            var entry = zip.GetEntry(entryPath);
            Stream stream = new ZipArchiveEntryStream(zip, entry);
            if (prog != null)
            {
                stream = new ProgressStream(stream, entry.Length, prog);
            }

            return stream;
        }

        public static Stream GetFile(FileInfo file, string entryPath)
        {
            return GetFile(file, entryPath, null);
        }

        public static Stream GetFile(string fileName, string entryPath, IProgress prog)
        {
            return GetFile(new FileInfo(fileName), entryPath, prog);
        }

        public static Stream GetFile(string fileName, string entryPath)
        {
            return GetFile(fileName, entryPath, null);
        }

        /// <summary>
        /// Enumerates all of the entires in a single Zip archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip">A zipfile stream</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        /// <returns>A lazy collection of <typeparamref name="ZipArchiveEntry"/> objects.</returns>
        public static IEnumerable<CompressedFileInfo> Entries(this ZipArchive zip, IProgress prog)
        {
            var i = 0;
            foreach (var entry in zip.Entries)
            {
                prog.Report(i++, zip.Entries.Count);
                yield return new CompressedFileInfo(entry);
            }

            prog.Report(i, zip.Entries.Count);
        }

        public static IEnumerable<CompressedFileInfo> Entries(this ZipArchive zip)
        {
            return zip.Entries(null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream zipStream, IProgress prog)
        {
            using var zip = new ZipArchive(zipStream);
            foreach (var entry in zip.Entries(prog))
            {
                yield return entry;
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream zipStream)
        {
            return Entries(zipStream, null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo zipFile, IProgress prog)
        {
            using var stream = zipFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            foreach (var entry in Entries(stream, prog))
            {
                yield return entry;
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo zipFile)
        {
            return Entries(zipFile, null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(string zipFileName, IProgress prog)
        {
            return Entries(new FileInfo(zipFileName), prog);
        }

        public static IEnumerable<CompressedFileInfo> Entries(string zipFileName)
        {
            return Entries(zipFileName, null);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            var i = 0;
            foreach (var entry in zip.Entries)
            {
                prog.Report(i++, zip.Entries.Count);
                try
                {
                    var fileName = entry.FullName;
                    if (fileName.StartsWith(entryPrefix, StringComparison.InvariantCulture))
                    {
                        fileName = fileName.Remove(0, entryPrefix.Length);
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
            }

            prog.Report(i, zip.Entries.Count);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            zip.Decompress(outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            zip.Decompress(outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, bool overwrite)
        {
            zip.Decompress(outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            zip.Decompress(outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, IProgress prog)
        {
            zip.Decompress(outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory, string entryPrefix)
        {
            zip.Decompress(outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(this ZipArchive zip, DirectoryInfo outputDirectory)
        {
            zip.Decompress(outputDirectory, string.Empty, true, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            using var zip = new ZipArchive(stream);
            zip.Decompress(outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            stream.Decompress(outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            stream.Decompress(outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, IProgress prog)
        {
            stream.Decompress(outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            stream.Decompress(outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, bool overwrite)
        {
            stream.Decompress(outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, string entryPrefix)
        {
            stream.Decompress(outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory)
        {
            stream.Decompress(outputDirectory, string.Empty, true, null);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            using var stream = zipFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Decompress(outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            Decompress(zipFile, outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            Decompress(zipFile, outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, IProgress prog)
        {
            Decompress(zipFile, outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            Decompress(zipFile, outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, bool overwrite)
        {
            Decompress(zipFile, outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, string entryPrefix)
        {
            Decompress(zipFile, outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory)
        {
            Decompress(zipFile, outputDirectory, string.Empty, true, null);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, string entryPrefix, bool overwrite, IProgress prog)
        {
            stream.Decompress(new DirectoryInfo(outputDirectoryName), entryPrefix, overwrite, prog);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            stream.Decompress(outputDirectoryName, string.Empty, overwrite, prog);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, string entryPrefix, IProgress prog)
        {
            stream.Decompress(outputDirectoryName, entryPrefix, true, prog);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, IProgress prog)
        {
            stream.Decompress(outputDirectoryName, string.Empty, true, prog);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, string entryPrefix, bool overwrite)
        {
            stream.Decompress(outputDirectoryName, entryPrefix, overwrite, null);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, bool overwrite)
        {
            stream.Decompress(outputDirectoryName, string.Empty, overwrite, null);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, string entryPrefix)
        {
            stream.Decompress(outputDirectoryName, entryPrefix, true, null);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName)
        {
            stream.Decompress(outputDirectoryName, string.Empty, true, null);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, string entryPrefix, bool overwrite, IProgress prog)
        {
            Decompress(zipFile, new DirectoryInfo(outputDirectoryName), entryPrefix, overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            Decompress(zipFile, outputDirectoryName, string.Empty, overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, string entryPrefix, IProgress prog)
        {
            Decompress(zipFile, outputDirectoryName, entryPrefix, true, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, IProgress prog)
        {
            Decompress(zipFile, outputDirectoryName, string.Empty, true, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, string entryPrefix, bool overwrite)
        {
            Decompress(zipFile, outputDirectoryName, entryPrefix, overwrite, null);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, bool overwrite)
        {
            Decompress(zipFile, outputDirectoryName, string.Empty, overwrite, null);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, string entryPrefix)
        {
            Decompress(zipFile, outputDirectoryName, entryPrefix, true, null);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName)
        {
            Decompress(zipFile, outputDirectoryName, string.Empty, true, null);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            Decompress(new FileInfo(zipFileName), outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            Decompress(zipFileName, outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            Decompress(zipFileName, outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, IProgress prog)
        {
            Decompress(zipFileName, outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            Decompress(zipFileName, outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, bool overwrite)
        {
            Decompress(zipFileName, outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, string entryPrefix)
        {
            Decompress(zipFileName, outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory)
        {
            Decompress(zipFileName, outputDirectory, string.Empty, true, null);
        }

        public static void Decompress(string zipFileName, string outputDiretoryName, string entryPrefix, bool overwrite, IProgress prog)
        {
            Decompress(zipFileName, new DirectoryInfo(outputDiretoryName), entryPrefix, overwrite, prog);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            Decompress(zipFileName, outputDirectoryName, string.Empty, overwrite, prog);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, string entryPrefix, IProgress prog)
        {
            Decompress(zipFileName, outputDirectoryName, entryPrefix, true, prog);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, IProgress prog)
        {
            Decompress(zipFileName, outputDirectoryName, string.Empty, true, prog);
        }

        public static void Decompress(string zipFileName, string outputDiretoryName, string entryPrefix, bool overwrite)
        {
            Decompress(zipFileName, outputDiretoryName, entryPrefix, overwrite, null);
        }

        public static void Decompress(string zipFileName, string outputDiretoryName, bool overwrite)
        {
            Decompress(zipFileName, outputDiretoryName, string.Empty, overwrite, null);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, string entryPrefix)
        {
            Decompress(zipFileName, outputDirectoryName, entryPrefix, true, null);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName)
        {
            Decompress(zipFileName, outputDirectoryName, string.Empty, true, null);
        }
    }
}