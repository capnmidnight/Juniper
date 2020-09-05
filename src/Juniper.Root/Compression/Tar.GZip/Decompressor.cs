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
        /// <param name="packageFileName">The .unitypackage file to read</param>
        /// <returns>A lazy collection of filenames.</returns>
        public static IEnumerable<CompressedFileInfo> UnityPackageEntries(string packageFileName)
        {
            if (packageFileName is null)
            {
                throw new ArgumentNullException(nameof(packageFileName));
            }

            if (packageFileName.Length == 0)
            {
                throw new ArgumentException("Value must not be an empty string.", nameof(packageFileName));
            }

            return UnityPackageEntries(new FileInfo(packageFileName));
        }

        public static IEnumerable<CompressedFileInfo> UnityPackageEntries(FileInfo packageFile)
        {
            if (packageFile is null)
            {
                throw new ArgumentNullException(nameof(packageFile));
            }

            if (!packageFile.Exists)
            {
                throw new FileNotFoundException("File not found! " + packageFile.FullName, packageFile.FullName);
            }

            using var tar = Open(packageFile);
            var paths = new Dictionary<string, string>();
            var sizes = new Dictionary<string, long>();
            foreach (var entry in tar.Entries)
            {
                var parts = entry.FullName.Split('/');
                if (parts.Length == 2)
                {
                    var guid = parts[0];
                    var item = parts[1];
                    if (item.Equals("asset", StringComparison.InvariantCulture))
                    {
                        sizes[guid] = item.Length;
                    }
                    else if (item.Equals("pathname", StringComparison.InvariantCulture))
                    {
                        using var stream = entry.Open();
                        using var streamReader = new StreamReader(stream);
                        paths[guid] = streamReader.ReadLine();
                    }

                    if (paths.ContainsKey(guid)
                        && sizes.ContainsKey(guid))
                    {
                        var path = paths[guid];
                        var size = sizes[guid];
                        paths.Remove(guid);
                        sizes.Remove(guid);
                        yield return new CompressedFileInfo(path, true, size);
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

        public static IEnumerable<CompressedFileInfo> Entries(Stream stream, IProgress prog = null)
        {
            if (stream is GZipStream gzStream)
            {
                using var tar = new TarArchive(gzStream);
                return tar.Entries(prog);
            }
            else
            {
                using var gzStream2 = new GZipStream(stream, CompressionMode.Decompress);
                using var tar = new TarArchive(gzStream2);
                return tar.Entries(prog);
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
    }
}