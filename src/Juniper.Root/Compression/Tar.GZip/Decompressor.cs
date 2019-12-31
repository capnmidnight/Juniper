using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Juniper.Progress;

namespace Juniper.Compression.Tar.GZip
{
    /// <summary>
    /// Functions for dealing with TarGz files.
    /// </summary>
    public static class Decompressor
    {
        /// <summary>
        /// Find all of the UnityPackage files in all subdirectories of the given root
        /// search directories.
        /// </summary>
        /// <param name="searchPaths"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindUnityPackages(params string[] searchPaths)
        {
            return from path in searchPaths
                   where Directory.Exists(path)
                   from file in Directory.GetFiles(path, "*.unitypackage", SearchOption.AllDirectories)
                   select file;
        }

        /// <summary>
        /// Read the names of all the files contained in a .unitypackage file.
        /// </summary>
        /// <param name="inputPackageFile">The .unitypackage file to read</param>
        /// <returns>A lazy collection of filenames.</returns>
        public static IEnumerable<CompressedFileInfo> UnityPackageEntries(string inputPackageFile)
        {
            if (!File.Exists(inputPackageFile))
            {
                throw new FileNotFoundException("File not found! " + inputPackageFile, inputPackageFile);
            }

            return UnityPackageEntries2();

            IEnumerable<CompressedFileInfo> UnityPackageEntries2()
            {
                using (var tar = Open(inputPackageFile))
                {
                    var fileSize = 0L;
                    foreach (var item in tar.Entries)
                    {
                        if (item.FullName.EndsWith("/asset", StringComparison.InvariantCulture))
                        {
                            fileSize = item.Length;
                        }
                        else if (item.FullName.EndsWith("/pathname", StringComparison.InvariantCulture))
                        {
                            using (var streamReader = new StreamReader(item.Open()))
                            {
                                var path = streamReader.ReadLine();
                                yield return new CompressedFileInfo(path, true, fileSize);
                            }
                        }
                    }
                }
            }
        }

        public static TarArchive Open(FileInfo file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }
            else
            {
                using (var stream = new GZipStream(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), CompressionMode.Decompress))
                {
                    return new TarArchive(stream);
                }
            }
        }

        public static TarArchive Open(string fileName)
        {
            return Open(new FileInfo(fileName));
        }

        /// <summary>
        /// Retrieves a single file from a tar file.
        /// </summary>
        /// <param name="fileName">The tar file in which to find the file.</param>
        /// <param name="entryPath">The file to find in the tar file.</param>
        /// <param name="copyTo">A stream to which to copy the file, before closing the tarGzFile.</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        public static void CopyFile(this TarArchive tar, string entryPath, Stream copyTo, IProgress prog)
        {
            var entry = tar.GetEntry(entryPath);
            if (entry == null)
            {
                throw new FileNotFoundException($"Could not find file {entryPath} in tar file.");
            }

            using (var fileStream = entry.Open())
            {
                var progStream = new ProgressStream(fileStream, entry.Length, prog);
                progStream.CopyTo(copyTo);
            }
        }

        public static void CopyFile(this TarArchive tar, string entryPath, Stream copyTo)
        {
            tar.CopyFile(entryPath, copyTo, null);
        }

        public static void CopyFile(this TarArchive tar, string entryPath, FileInfo copyToFile, IProgress prog)
        {
            copyToFile.Directory.Create();
            using (var copyTo = copyToFile.Create())
            {
                tar.CopyFile(entryPath, copyTo, prog);
            }
        }

        public static void CopyFile(this TarArchive tar, string entryPath, FileInfo copyToFile)
        {
            tar.CopyFile(entryPath, copyToFile, null);
        }

        public static void CopyFile(this TarArchive tar, string entryPath, string copyToFileName, IProgress prog)
        {
            tar.CopyFile(entryPath, new FileInfo(copyToFileName), prog);
        }

        public static void CopyFile(this TarArchive tar, string entryPath, string copyToFileName)
        {
            tar.CopyFile(entryPath, copyToFileName, null);
        }

        public static void CopyFile(FileInfo file, string entryPath, Stream copyTo, IProgress prog)
        {
            using (var tar = Open(file))
            {
                tar.CopyFile(entryPath, copyTo, prog);
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

        public static Stream GetFile(this TarArchive tar, string entryPath, IProgress prog)
        {
            var entry = tar.GetEntry(entryPath);
            var stream = entry.Open();
            if (prog != null)
            {
                stream = new ProgressStream(stream, entry.Length, prog);
            }

            return stream;
        }

        public static Stream GetFile(this TarArchive tar, string entryPath)
        {
            return tar.GetFile(entryPath, null);
        }

        public static Stream GetFile(FileInfo file, string entryPath, IProgress prog)
        {
            var tar = Open(file);
            return tar.GetFile(entryPath, prog);
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
        /// <param name="tar">A zipfile stream</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        /// <returns>A lazy collection of <typeparamref name="ZipArchiveEntry"/> objects.</returns>
        public static IEnumerable<CompressedFileInfo> Entries(this TarArchive tar, IProgress prog)
        {
            var i = 0;
            foreach (var entry in tar.Entries)
            {
                prog.Report(i++, tar.Entries.Count);
                yield return new CompressedFileInfo(entry);
            }

            prog.Report(i, tar.Entries.Count);
        }

        public static IEnumerable<CompressedFileInfo> Entries(this TarArchive tar)
        {
            return tar.Entries(null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream tarGzStream, IProgress prog)
        {
            using (var tar = new TarArchive(new GZipStream(tarGzStream, CompressionMode.Decompress)))
            {
                foreach (var entry in tar.Entries(prog))
                {
                    yield return entry;
                }
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream tarGzStream)
        {
            return Entries(tarGzStream, null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo tarGzFile, IProgress prog)
        {
            using (var stream = tarGzFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                foreach (var entry in Entries(stream, prog))
                {
                    yield return entry;
                }
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(FileInfo tarGzFile)
        {
            return Entries(tarGzFile, null);
        }

        public static IEnumerable<CompressedFileInfo> Entries(string tarGzFileName, IProgress prog)
        {
            return Entries(new FileInfo(tarGzFileName), prog);
        }

        public static IEnumerable<CompressedFileInfo> Entries(string tarGzFileName)
        {
            return Entries(tarGzFileName, null);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            var i = 0;
            foreach (var entry in tar.Entries)
            {
                prog.Report(i++, tar.Entries.Count);
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
                            using (var outputStream = outputFile.Create())
                            using (var inputStream = entry.Open())
                            {
                                inputStream.CopyTo(outputStream);
                            }
                        }
                    }
                }
                catch { }
            }

            prog.Report(i, tar.Entries.Count);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            tar.Decompress(outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            tar.Decompress(outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, bool overwrite)
        {
            tar.Decompress(outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            tar.Decompress(outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, IProgress prog)
        {
            tar.Decompress(outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory, string entryPrefix)
        {
            tar.Decompress(outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(this TarArchive tar, DirectoryInfo outputDirectory)
        {
            tar.Decompress(outputDirectory, string.Empty, true, null);
        }

        public static void Decompress(this Stream stream, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            using (var tar = new TarArchive(new GZipStream(stream, CompressionMode.Decompress)))
            {
                tar.Decompress(outputDirectory, entryPrefix, overwrite, prog);
            }
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

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            using (var stream = tarGzFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Decompress(outputDirectory, entryPrefix, overwrite, prog);
            }
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            Decompress(tarGzFile, outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            Decompress(tarGzFile, outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, IProgress prog)
        {
            Decompress(tarGzFile, outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            Decompress(tarGzFile, outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, bool overwrite)
        {
            Decompress(tarGzFile, outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory, string entryPrefix)
        {
            Decompress(tarGzFile, outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(FileInfo tarGzFile, DirectoryInfo outputDirectory)
        {
            Decompress(tarGzFile, outputDirectory, string.Empty, true, null);
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

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, string entryPrefix, bool overwrite, IProgress prog)
        {
            Decompress(tarGzFile, new DirectoryInfo(outputDirectoryName), entryPrefix, overwrite, prog);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            Decompress(tarGzFile, outputDirectoryName, string.Empty, overwrite, prog);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, string entryPrefix, IProgress prog)
        {
            Decompress(tarGzFile, outputDirectoryName, entryPrefix, true, prog);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, IProgress prog)
        {
            Decompress(tarGzFile, outputDirectoryName, string.Empty, true, prog);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, string entryPrefix, bool overwrite)
        {
            Decompress(tarGzFile, outputDirectoryName, entryPrefix, overwrite, null);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, bool overwrite)
        {
            Decompress(tarGzFile, outputDirectoryName, string.Empty, overwrite, null);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName, string entryPrefix)
        {
            Decompress(tarGzFile, outputDirectoryName, entryPrefix, true, null);
        }

        public static void Decompress(FileInfo tarGzFile, string outputDirectoryName)
        {
            Decompress(tarGzFile, outputDirectoryName, string.Empty, true, null);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite, IProgress prog)
        {
            Decompress(new FileInfo(tarGzFileName), outputDirectory, entryPrefix, overwrite, prog);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, bool overwrite, IProgress prog)
        {
            Decompress(tarGzFileName, outputDirectory, string.Empty, overwrite, prog);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, string entryPrefix, IProgress prog)
        {
            Decompress(tarGzFileName, outputDirectory, entryPrefix, true, prog);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, IProgress prog)
        {
            Decompress(tarGzFileName, outputDirectory, string.Empty, true, prog);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, string entryPrefix, bool overwrite)
        {
            Decompress(tarGzFileName, outputDirectory, entryPrefix, overwrite, null);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, bool overwrite)
        {
            Decompress(tarGzFileName, outputDirectory, string.Empty, overwrite, null);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory, string entryPrefix)
        {
            Decompress(tarGzFileName, outputDirectory, entryPrefix, true, null);
        }

        public static void Decompress(string tarGzFileName, DirectoryInfo outputDirectory)
        {
            Decompress(tarGzFileName, outputDirectory, string.Empty, true, null);
        }

        public static void Decompress(string tarGzFileName, string outputDiretoryName, string entryPrefix, bool overwrite, IProgress prog)
        {
            Decompress(tarGzFileName, new DirectoryInfo(outputDiretoryName), entryPrefix, overwrite, prog);
        }

        public static void Decompress(string tarGzFileName, string outputDirectoryName, bool overwrite, IProgress prog)
        {
            Decompress(tarGzFileName, outputDirectoryName, string.Empty, overwrite, prog);
        }

        public static void Decompress(string tarGzFileName, string outputDirectoryName, string entryPrefix, IProgress prog)
        {
            Decompress(tarGzFileName, outputDirectoryName, entryPrefix, true, prog);
        }

        public static void Decompress(string tarGzFileName, string outputDirectoryName, IProgress prog)
        {
            Decompress(tarGzFileName, outputDirectoryName, string.Empty, true, prog);
        }

        public static void Decompress(string tarGzFileName, string outputDiretoryName, string entryPrefix, bool overwrite)
        {
            Decompress(tarGzFileName, outputDiretoryName, entryPrefix, overwrite, null);
        }

        public static void Decompress(string tarGzFileName, string outputDiretoryName, bool overwrite)
        {
            Decompress(tarGzFileName, outputDiretoryName, string.Empty, overwrite, null);
        }

        public static void Decompress(string tarGzFileName, string outputDirectoryName, string entryPrefix)
        {
            Decompress(tarGzFileName, outputDirectoryName, entryPrefix, true, null);
        }

        public static void Decompress(string tarGzFileName, string outputDirectoryName)
        {
            Decompress(tarGzFileName, outputDirectoryName, string.Empty, true, null);
        }
    }
}