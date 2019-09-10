using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using Juniper.Progress;
using Juniper.Streams;

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
        public static void CopyFile(this ZipFile zip, string entryPath, Stream copyTo, IProgress prog = null)
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

        public static void CopyFile(this ZipFile zip, string entryPath, FileInfo copyToFile, IProgress prog = null)
        {
            copyToFile.Directory.Create();
            using (var copyTo = copyToFile.Create())
            {
                zip.CopyFile(entryPath, copyTo, prog);
            }
        }

        public static void CopyFile(this ZipFile zip, string entryPath, string copyToFileName, IProgress prog = null)
        {
            zip.CopyFile(entryPath, new FileInfo(copyToFileName), prog);
        }

        public static void CopyFile(FileInfo file, string entryPath, Stream copyTo, IProgress prog = null)
        {
            using (var zip = OpenZip(file))
            {
                zip.CopyFile(entryPath, copyTo, prog);
            }
        }

        public static void CopyFile(string fileName, string entryPath, Stream copyTo, IProgress prog = null)
        {
            CopyFile(new FileInfo(fileName), entryPath, copyTo, prog);
        }

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="fileName">The zip file in which to find the file.</param>
        /// <param name="entryPath">The file to find in the zip file.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        public static Stream GetFile(this ZipFile zip, string entryPath, IProgress prog = null)
        {
            var mem = new MemoryStream();
            zip.CopyFile(entryPath, mem, prog);
            mem.Flush();
            mem.Position = 0;
            return mem;
        }

        public static Stream GetFile(FileInfo file, string entryPath, IProgress prog = null)
        {
            using (var zip = OpenZip(file))
            {
                return zip.GetFile(entryPath, prog);
            }
        }

        public static Stream GetFile(string fileName, string entryPath, IProgress prog = null)
        {
            return GetFile(new FileInfo(fileName), entryPath, prog);
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip">A zipfile stream</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of <typeparamref name="ZipEntry"/> objects.</returns>
        public static IEnumerable<ZipEntry> Entries(this ZipFile zip, IProgress prog = null)
        {
            for (int i = 0, l = (int)zip.Count; i < l; ++i)
            {
                prog?.Report(i, l);
                yield return zip[i];
                prog?.Report(i + 1, l);
            }
        }

        public static IEnumerable<ZipEntry> Entries(Stream zipStream, IProgress progress = null)
        {
            var zip = new ZipFile(zipStream);
            return zip.Entries(progress);
        }

        public static IEnumerable<ZipEntry> Entries(FileInfo zipFile, IProgress prog = null)
        {
            using (var stream = zipFile.OpenRead())
            {
                return Entries(stream, prog);
            }
        }

        public static IEnumerable<ZipEntry> Entries(string zipFileName, IProgress prog = null)
        {
            return Entries(new FileInfo(zipFileName), prog);
        }

        public static IEnumerable<string> FileNames(FileInfo file)
        {
            foreach (var entry in Entries(file))
            {
                if (entry.IsFile)
                {
                    yield return entry.Name;
                }
            }
        }

        public static IEnumerable<string> FileNames(string fileName)
        {
            return FileNames(new FileInfo(fileName));
        }

        public static IEnumerable<string> DirectoryNames(FileInfo file)
        {
            foreach (var entry in Entries(file))
            {
                if (entry.IsDirectory)
                {
                    yield return entry.Name;
                }
            }
        }

        public static IEnumerable<string> DirectoryNames(string fileName)
        {
            return DirectoryNames(new FileInfo(fileName));
        }

        public static void Decompress(this ZipFile zip, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            for (int i = 0, l = (int)zip.Count; i < l; ++i)
            {
                prog?.Report(i, l);
                var entry = zip[i];
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

                if (outputFile != null)
                {
                    using (var outputStream = outputFile.Create())
                    using (var inputStream = zip.GetInputStream(entry))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
                prog?.Report(i + 1, l);
            }
        }

        public static void Decompress(this Stream stream, string outputDirectoryName, IProgress prog = null)
        {
            stream.Decompress(new DirectoryInfo(outputDirectoryName), prog);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            new ZipFile(stream).Decompress(outputDirectory, prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            using (var stream = zipFile.OpenRead())
            {
                stream.Decompress(outputDirectory, prog);
            }
        }

        public static void Decompress(FileInfo zipFile, string outputDirectoryName, IProgress prog = null)
        {
            Decompress(zipFile, new DirectoryInfo(outputDirectoryName), prog);
        }

        public static void Decompress(string zipFileName, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            Decompress(new FileInfo(zipFileName), outputDirectory, prog);
        }

        public static void Decompress(string zipFileName, string outputDiretoryName, IProgress prog = null)
        {
            Decompress(new FileInfo(zipFileName), new DirectoryInfo(outputDiretoryName), prog);
        }
    }
}