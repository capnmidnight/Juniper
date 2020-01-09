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
                using var tar = Open(inputPackageFile);
                var fileSize = 0L;
                foreach (var item in tar.Entries)
                {
                    if (item.FullName.EndsWith("/asset", StringComparison.InvariantCulture))
                    {
                        fileSize = item.Length;
                    }
                    else if (item.FullName.EndsWith("/pathname", StringComparison.InvariantCulture))
                    {
                        using var streamReader = new StreamReader(item.Open());
                        var path = streamReader.ReadLine();
                        yield return new CompressedFileInfo(path, true, fileSize);
                    }
                }
            }
        }

        public static TarArchive Open(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }

            using var stream = new GZipStream(file.OpenRead(), CompressionMode.Decompress);
            return new TarArchive(stream);
        }

        public static TarArchive Open(string fileName)
        {
            return Open(new FileInfo(fileName.ValidateFileName()));
        }

        /// <summary>
        /// Enumerates all of the entires in a single Zip archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar">A zipfile stream</param>
        /// <param name="prog">A prog tracking object, defaults to null (i.e. no prog tracking).</param>
        /// <returns>A lazy collection of <typeparamref name="ZipArchiveEntry"/> objects.</returns>
        public static IEnumerable<CompressedFileInfo> Entries(this TarArchive tar, IProgress prog = null)
        {
            if (tar is null)
            {
                throw new ArgumentNullException(nameof(tar));
            }

            var i = 0;
            foreach (var entry in tar.Entries)
            {
                prog.Report(i++, tar.Entries.Count);
                yield return new CompressedFileInfo(entry);
            }

            prog.Report(i, tar.Entries.Count);
        }

        public static IEnumerable<CompressedFileInfo> Entries(Stream tarGzStream, IProgress prog = null)
        {
            using var stream = new GZipStream(tarGzStream, CompressionMode.Decompress);
            using var tar = new TarArchive(stream);
            foreach (var entry in tar.Entries(prog))
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

            using var stream = file.OpenRead();
            foreach (var entry in Entries(stream, prog))
            {
                yield return entry;
            }
        }

        public static IEnumerable<CompressedFileInfo> Entries(string fileName, IProgress prog = null)
        {
            return Entries(new FileInfo(fileName.ValidateFileName()), prog);
        }
    }
}