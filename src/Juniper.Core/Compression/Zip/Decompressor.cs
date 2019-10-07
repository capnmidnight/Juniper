using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpZipLib.Zip;

using Juniper.Progress;

namespace Juniper.Compression.Zip
{
    /// <summary>
    /// Functions for dealing with Zip files.
    /// </summary>
    public static class Decompressor
    {
        /// <summary>
        /// Force SharpZipLib to use any code page that is available on the runtime system, rather
        /// than stubbornly insisting on trying to use IBM437, which is only available by default
        /// on Windows.
        /// </summary>
        static Decompressor()
        {
            ZipStrings.UseUnicode = true;
        }

        public static ZipFile OpenZip(FileInfo file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                return new ZipFile(file.OpenRead())
                {
                    IsStreamOwner = true
                };
            }
        }

        public static ZipFile OpenZip(string fileName)
        {
            return OpenZip(new FileInfo(fileName));
        }

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="fileName">The zip file in which to find the file.</param>
        /// <param name="entryPath">The file to find in the zip file.</param>
        /// <param name="copyTo">A stream to which to copy the file, before closing the zipFile.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        public static void CopyFile(this ZipFile zip, string entryPath, Stream copyTo, IProgress prog)
        {
            var entryIndex = zip.FindEntry(entryPath, true);
            if (entryIndex > -1)
            {
                var entry = zip[entryIndex];
                using (var fileStream = zip.GetInputStream(entry))
                {
                    var progStream = new ProgressStream(fileStream, entry.Size, prog);
                    progStream.CopyTo(copyTo);
                }
            }
            else
            {
                throw new FileNotFoundException($"Could not find file {entryPath} in zip file {zip.Name}", $"{zip.Name}::{entryPath}");
            }
        }

        public static void CopyFile(this ZipFile zip, string entryPath, Stream copyTo)
        {
            zip.CopyFile(entryPath, copyTo, null);
        }

        public static void CopyFile(this ZipFile zip, string entryPath, FileInfo copyToFile, IProgress prog)
        {
            copyToFile.Directory.Create();
            using (var copyTo = copyToFile.Create())
            {
                zip.CopyFile(entryPath, copyTo, prog);
            }
        }

        public static void CopyFile(this ZipFile zip, string entryPath, FileInfo copyToFile)
        {
            zip.CopyFile(entryPath, copyToFile, null);
        }

        public static void CopyFile(this ZipFile zip, string entryPath, string copyToFileName, IProgress prog)
        {
            zip.CopyFile(entryPath, new FileInfo(copyToFileName), prog);
        }

        public static void CopyFile(this ZipFile zip, string entryPath, string copyToFileName)
        {
            zip.CopyFile(entryPath, copyToFileName, null);
        }

        public static void CopyFile(FileInfo file, string entryPath, Stream copyTo, IProgress prog)
        {
            using (var zip = OpenZip(file))
            {
                zip.CopyFile(entryPath, copyTo, prog);
            }
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

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="fileName">The zip file in which to find the file.</param>
        /// <param name="entry">The file to find in the zip file.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        public static Stream GetFile(this ZipFile zip, ZipEntry entry, IProgress prog)
        {
            return new ProgressStream(zip.GetInputStream(entry), entry.Size, prog);
        }

        public static Stream GetFile(this ZipFile zip, ZipEntry entry)
        {
            return zip.GetFile(entry, null);
        }

        public static Stream GetFile(this ZipFile zip, string entryPath, IProgress prog)
        {
            return zip.GetFile(zip.GetEntry(entryPath), prog);
        }

        public static Stream GetFile(this ZipFile zip, string entryPath)
        {
            return zip.GetFile(entryPath, null);
        }

        public static Stream GetFile(FileInfo file, string entryPath, IProgress prog)
        {
            var zip = OpenZip(file);
            var entry = zip.GetEntry(entryPath);
            return zip.GetFile(entry, prog);
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
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip">A zipfile stream</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of <typeparamref name="ZipEntry"/> objects.</returns>
        public static IEnumerable<CompressedFileInfo> Entries(this ZipFile zip, IProgress prog)
        {
            for (int i = 0, l = (int)zip.Count; i < l; ++i)
            {
                prog.Report(i, l);
                yield return new CompressedFileInfo(zip[i]);
                prog.Report(i + 1, l);
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(this ZipFile zip)
        {
            return zip.Entries(null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream zipStream, IProgress progress)
        {
            using (var zip = new ZipFile(zipStream))
            {
                return zip.Entries(progress);
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream zipStream)
        {
            return Entries(zipStream, null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo zipFile, IProgress prog)
        {
            using (var stream = zipFile.OpenRead())
            {
                return Entries(stream, prog);
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

        public static void Decompress(this ZipFile zip, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            for (var i = 0L; i < zip.Count; ++i)
            {
                prog.Report(i, zip.Count);
                try
                {
                    var entry = zip[(int)i];
                    var outputPath = Path.Combine(outputDirectory.FullName, entry.Name);
                    FileInfo outputFile = null;
                    DirectoryInfo outputFileDirectory = null;
                    if (entry.IsDirectory)
                    {
                        outputFileDirectory = new DirectoryInfo(outputPath);
                    }
                    else if (entry.IsFile)
                    {
                        outputFile = new FileInfo(outputPath);
                        outputFileDirectory = outputFile.Directory;
                    }

                    outputFileDirectory.Create();

                    if (outputFile != null
                        && (overwrite || !outputFile.Exists))
                    {
                        using (var outputStream = outputFile.Create())
                        using (var inputStream = zip.GetInputStream(entry))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
                catch { }
                prog.Report(i + 1, zip.Count);
            }
        }

        public static void Decompress(this ZipFile zip, DirectoryInfo outputDirectory, bool overwrite)
        {
            zip.Decompress(outputDirectory, overwrite, null);
        }

        public static void Decompress(this ZipFile zip, DirectoryInfo outputDirectory, IProgress prog)
        {
            zip.Decompress(outputDirectory, true, prog);
        }

        public static void Decompress(this ZipFile zip, DirectoryInfo outputDirectory)
        {
            zip.Decompress(outputDirectory, true, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            using (var zip = new ZipFile(stream))
            {
                zip.Decompress(outputDirectory, overwrite, prog);
            }
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, IProgress prog)
        {
            stream.Decompress(outputDirectory, true, prog);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, bool overwrite)
        {
            stream.Decompress(outputDirectory, overwrite, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory)
        {
            stream.Decompress(outputDirectory, true, null);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            using (var stream = zipFile.OpenRead())
            {
                stream.Decompress(outputDirectory, overwrite, prog);
            }
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, IProgress prog)
        {
            Decompress(zipFile, outputDirectory, true, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, bool overwrite)
        {
            Decompress(zipFile, outputDirectory, overwrite, null);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory)
        {
            Decompress(zipFile, outputDirectory, true, null);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            stream.Decompress(new DirectoryInfo(outputDirectoryName), overwrite, prog);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, IProgress prog)
        {
            stream.Decompress(outputDirectoryName, true, prog);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, bool overwrite)
        {
            stream.Decompress(outputDirectoryName, overwrite, null);
        }

        public static void Decompress(this Stream stream, string outputDirectoryName)
        {
            stream.Decompress(outputDirectoryName, true, null);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            Decompress(zipFile, new DirectoryInfo(outputDirectoryName), overwrite, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, IProgress prog)
        {
            Decompress(zipFile, outputDirectoryName, true, prog);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, bool overwrite)
        {
            Decompress(zipFile, outputDirectoryName, overwrite, null);
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName)
        {
            Decompress(zipFile, outputDirectoryName, true, null);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            Decompress(new FileInfo(zipFileName), outputDirectory, overwrite, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, IProgress prog)
        {
            Decompress(zipFileName, outputDirectory, true, prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, bool overwrite)
        {
            Decompress(zipFileName, outputDirectory, overwrite, null);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory)
        {
            Decompress(zipFileName, outputDirectory, true, null);
        }

        public static void Decompress(string zipFileName, string outputDiretoryName, bool overwrite, IProgress prog)
        {
            Decompress(zipFileName, new DirectoryInfo(outputDiretoryName), overwrite, prog);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName, IProgress prog)
        {
            Decompress(zipFileName, outputDirectoryName, true, prog);
        }

        public static void Decompress(string zipFileName, string outputDiretoryName, bool overwrite)
        {
            Decompress(zipFileName, outputDiretoryName, overwrite, null);
        }

        public static void Decompress(string zipFileName, string outputDirectoryName)
        {
            Decompress(zipFileName, outputDirectoryName, true, null);
        }
    }
}