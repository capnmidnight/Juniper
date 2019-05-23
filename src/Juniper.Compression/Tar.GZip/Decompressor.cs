using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

using Juniper.Collections;

namespace Juniper.Compression.Tar.GZip
{
    /// <summary>
    /// Functions for dealing with TGZ files.
    /// </summary>
    public static class Decompressor
    {
        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static IEnumerable<T> Entries<T>(this TarInputStream tar, Func<TarEntry, TarInputStream, bool> checkEntry, Func<TarEntry, TarInputStream, T> eachEntry)
        {
            TarEntry entry;
            while ((entry = tar.GetNextEntry()) != null)
            {
                if (checkEntry(entry, tar))
                {
                    yield return eachEntry(entry, tar);
                }
            }
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntry objects, as filtered by <paramref name="checkEntry"/>.</returns>
        public static IEnumerable<TarEntry> Entries(this TarInputStream tar, Func<TarEntry, TarInputStream, bool> checkEntry)
        {
            return tar.Entries(
                checkEntry,
                (entry, _) => entry);
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <returns>A lazy collection of TarEntry objects.</returns>
        public static IEnumerable<TarEntry> Entries(this TarInputStream tar)
        {
            return tar.Entries(
                (_, __) => true,
                (entry, _) => entry);
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a TGZ file.
        /// </summary>
        /// <param name="inputTgzFile">A file-path to the TGZ file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        private static IEnumerable<T> WithTGZ<T>(string inputTgzFile, Func<TarEntry, TarInputStream, bool> checkEntry, Func<TarEntry, TarInputStream, T> eachEntry)
        {
            if (!File.Exists(inputTgzFile))
            {
                throw new FileNotFoundException("File not found! " + inputTgzFile, inputTgzFile);
            }
            else
            {
                using (var gzip = new GZipInputStream(File.OpenRead(inputTgzFile)))
                using (var tar = new TarInputStream(gzip))
                {
                    tar.IsStreamOwner = true;
                    return tar.Entries(checkEntry, eachEntry);
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a TGZ file.
        /// </summary>
        /// <param name="inputTgzFile">A file-path to the TGZ file to scan.</param>
        /// <returns>A lazy collection of TarEntries</returns>
        public static IEnumerable<TarEntry> Entries(string inputTgzFile)
        {
            return WithTGZ(
                inputTgzFile,
                null,
                (entry, _) => entry);
        }


        /// <summary>
        /// Find all of the UnityPackage files in all subdirectories of the given root
        /// search directories.
        /// </summary>
        /// <param name="searchPaths"></param>
        /// <returns></returns>
        public static IEnumerable<string> UnityPackageFiles(params string[] searchPaths)
        {
            return from path in searchPaths
                   from subDir in DirectoryExt.RecurseDirectories(path)
                   from file in Directory.GetFiles(subDir, "*.unitypackage")
                   select file;
        }

        /// <summary>
        /// Read the names of all the files contained in a .unitypackage file.
        /// </summary>
        /// <param name="inputPackageFile">The .unitypackage file to read</param>
        /// <returns>A lazy collection of filenames.</returns>
        public static FileTree UnityPackageContent(string inputPackageFile)
        {
            if (!File.Exists(inputPackageFile))
            {
                throw new FileNotFoundException("File not found! " + inputPackageFile, inputPackageFile);
            }
            else
            {
                using (var inStream = new GZipInputStream(File.OpenRead(inputPackageFile)))
                using (var tar = new TarInputStream(inStream))
                {
                    tar.IsStreamOwner = true;
                    var tree = new FileTree();
                    foreach (var entry in tar.Entries())
                    {
                        if (!entry.IsDirectory && entry.Name == "pathname")
                        {
                            var mem = new MemoryStream((int)entry.Size);
                            tar.CopyEntryContents(mem);
                            mem.Flush();
                            mem.Position = 0;
                            var streamReader = new StreamReader(mem);
                            var name = streamReader.ReadLine();
                            tree.AddPath(name, false);
                        }
                    }

                    return tree;
                }
            }
        }
    }
}