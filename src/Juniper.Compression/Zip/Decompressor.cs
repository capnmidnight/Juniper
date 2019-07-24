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
                using (var progStream = new ProgressStream(fileStream, entry.Size, prog))
                {
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
            using (var copyTo = copyToFile.OpenWrite())
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
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var fileStream = file.OpenRead())
                using (var zip = new ZipFile(fileStream))
                {
                    zip.IsStreamOwner = true;
                    zip.CopyFile(entryPath, copyTo, prog);
                }
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
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var fileStream = file.OpenRead())
                using (var zip = new ZipFile(fileStream))
                {
                    zip.IsStreamOwner = true;
                    return zip.GetFile(entryPath, prog);
                }
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
        public static IEnumerable<ZipEntry> Entries(this ZipInputStream zip, IProgress prog = null)
        {
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null)
            {
                yield return entry;
            }
        }

        public static IEnumerable<Tuple<ZipInputStream, ZipEntry>> Entries(FileInfo zipFile, IProgress prog = null)
        {
            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("File not found!", zipFile.FullName);
            }
            else
            {
                using (var zip = new ZipInputStream(zipFile.OpenRead()))
                {
                    zip.IsStreamOwner = true;
                    foreach (var entry in zip.Entries(prog))
                    {
                        yield return new Tuple<ZipInputStream, ZipEntry>(zip, entry);
                    }
                }
            }
        }

        public static IEnumerable<Tuple<ZipInputStream, ZipEntry>> Entries(string zipFileName, IProgress prog = null)
        {
            return Entries(new FileInfo(zipFileName), prog);
        }

        public static void Decompress(this ZipInputStream zip, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            foreach (var entry in zip.Entries(prog))
            {
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
                    using (var outputStream = outputFile.OpenWrite())
                    {
                        zip.CopyTo(outputStream);
                    }
                }
            }
        }

        public static void Decompress(this ZipInputStream zip, string outputDirectoryName, IProgress prog = null)
        {
            zip.Decompress(new DirectoryInfo(outputDirectoryName), prog);
        }

        public static void Decompress(FileInfo zipFile, DirectoryInfo outputDirectory, IProgress prog = null)
        {
            if (!zipFile.Exists)
            {
                throw new FileNotFoundException("File not found!", zipFile.FullName);
            }
            else
            {
                using (var zip = new ZipInputStream(zipFile.OpenRead()))
                {
                    zip.IsStreamOwner = true;
                    zip.Decompress(outputDirectory, prog);
                }
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