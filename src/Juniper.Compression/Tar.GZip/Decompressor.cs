using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

using Juniper.Collections;
using Juniper.Progress;

namespace Juniper.Compression.Tar.GZip
{
    /// <summary>
    /// Functions for dealing with TGZ files.
    /// </summary>
    public static class Decompressor
    {
        /// <summary>
        /// Enumerates all of the entires in a single Tar archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static IEnumerable<T> Select<T>(this TarInputStream tar, Func<TarEntry, TarInputStream, bool> checkEntry, Func<TarEntry, TarInputStream, T> eachEntry)
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
        /// Lists all of the entries (both files and directories) in a Tar file.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        private static IEnumerable<T> Select<T>(FileInfo inputTarFile, Func<TarEntry, TarInputStream, bool> checkEntry, Func<TarEntry, TarInputStream, T> eachEntry)
        {
            if (!inputTarFile.Exists)
            {
                throw new FileNotFoundException("File not found! " + inputTarFile.FullName, inputTarFile.FullName);
            }
            else
            {
                using (var tar = new TarInputStream(inputTarFile.OpenRead()))
                {
                    tar.IsStreamOwner = true;
                    return tar.Select(checkEntry, eachEntry);
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Tar file.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        private static IEnumerable<T> Select<T>(string inputTarFile, Func<TarEntry, TarInputStream, bool> checkEntry, Func<TarEntry, TarInputStream, T> eachEntry)
        {
            return Select(new FileInfo(inputTarFile), checkEntry, eachEntry);
        }

        /// <summary>
        /// Enumerates all of the entires in a single Tar archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntry objects, as filtered by <paramref name="checkEntry"/>.</returns>
        public static IEnumerable<TarEntry> Select(this TarInputStream tar, Func<TarEntry, TarInputStream, bool> checkEntry)
        {
            TarEntry entry;
            while ((entry = tar.GetNextEntry()) != null)
            {
                if (checkEntry(entry, tar))
                {
                    yield return entry;
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Tar file.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntries</returns>
        public static IEnumerable<TarEntry> Select(FileInfo inputTarFile, Func<TarEntry, TarInputStream, bool> checkEntry)
        {
            if (!inputTarFile.Exists)
            {
                throw new FileNotFoundException("File not found! " + inputTarFile.FullName, inputTarFile.FullName);
            }
            else
            {
                using (var tar = new TarInputStream(inputTarFile.OpenRead()))
                {
                    tar.IsStreamOwner = true;
                    return tar.Select(checkEntry);
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Tar file.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntries</returns>
        public static IEnumerable<TarEntry> Select(string inputTarFile, Func<TarEntry, TarInputStream, bool> checkEntry)
        {
            return Select(new FileInfo(inputTarFile), checkEntry);
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static void ForEach(this TarInputStream tar, Action<TarEntry, TarInputStream> eachEntry)
        {
            TarEntry entry;
            while ((entry = tar.GetNextEntry()) != null)
            {
                eachEntry(entry, tar);
            }
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static void ForEach<T>(string inputTarFile, T state, Action<T, TarEntry, TarInputStream> eachEntry, Action<Exception> error = null)
        {
            if (!File.Exists(inputTarFile))
            {
                throw new FileNotFoundException("File not found! " + inputTarFile, inputTarFile);
            }
            else
            {
                using (var tar = new TarInputStream(File.OpenRead(inputTarFile)))
                {
                    tar.IsStreamOwner = true;
                    TarEntry entry;
                    while ((entry = tar.GetNextEntry()) != null)
                    {
                        try
                        {
                            eachEntry(state, entry, tar);
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception exp)
                        {
                            error?.Invoke(exp);
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all of the entires in a single Tar archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="tar"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntry objects, as filtered by <paramref name="checkEntry"/>.</returns>
        public static IEnumerable<TarEntry> Select(this TarInputStream tar)
        {
            TarEntry entry;
            while ((entry = tar.GetNextEntry()) != null)
            {
                yield return entry;
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Tar file.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntries</returns>
        public static IEnumerable<TarEntry> Select(FileInfo inputTarFile)
        {
            if (!inputTarFile.Exists)
            {
                throw new FileNotFoundException("File not found! " + inputTarFile.FullName, inputTarFile.FullName);
            }
            else
            {
                using (var tar = new TarInputStream(inputTarFile.OpenRead()))
                {
                    tar.IsStreamOwner = true;
                    return tar.Select();
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Tar file.
        /// </summary>
        /// <param name="inputTarFile">A file-path to the Tar file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of TarEntries</returns>
        public static IEnumerable<TarEntry> Select(string inputTarFile)
        {
            return Select(new FileInfo(inputTarFile));
        }

        /// <summary>
        /// Dump the contents of a tar file out to disk.
        /// </summary>
        /// <param name="inputTarFile">The tar file to decompress</param>
        /// <param name="outputDirectory">The location to which to dump the files.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        public static void Decompress(string inputTarFile, string outputDirectory, IProgress prog = null, Action<Exception> error = null)
        {
            prog?.Report(0);
            if (!File.Exists(inputTarFile))
            {
                throw new FileNotFoundException("File not found! " + inputTarFile, inputTarFile);
            }
            else
            {
                ForEach(inputTarFile, outputDirectory, Decompress, error);
            }
        }

        private static void Decompress(string oDir, TarEntry tarEntry, TarInputStream tarStream)
        {
            var fullTarToPath = Path.Combine(oDir, tarEntry.Name);
            if (tarEntry.IsDirectory)
            {
                DirectoryExt.CreateDirectory(fullTarToPath);
            }
            else if (!File.Exists(fullTarToPath)
                || FileExt.TryDelete(fullTarToPath))
            {
                var directoryName = Path.GetDirectoryName(fullTarToPath);
                if (directoryName.Length > 0)
                {
                    DirectoryExt.CreateDirectory(directoryName);
                }

                tarStream.CopyTo(File.Create(fullTarToPath));
            }
        }

        /// <summary>
        /// Get the names of all the directories in the zip file.
        /// </summary>
        /// <param name="inputTarFile"></param>
        /// <returns></returns>
        public static IEnumerable<string> DirectoryNames(string inputTarFile)
        {
            return Select(inputTarFile,
                (entry, _) => entry.IsDirectory,
                (entry, _) => entry.Name);
        }

        /// <summary>
        /// Get the names of all the files in the zip file.
        /// </summary>
        /// <param name="inputTarFile"></param>
        /// <returns></returns>
        public static IEnumerable<string> FileNames(string inputTarFile)
        {
            return Select(inputTarFile,
                (entry, _) => !entry.IsDirectory,
                (entry, _) => entry.Name);
        }

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
        public static IEnumerable<string> UnityPackageFiles(string inputPackageFile)
        {
            if (!File.Exists(inputPackageFile))
            {
                throw new FileNotFoundException("File not found! " + inputPackageFile, inputPackageFile);
            }
            using (var inputStream = new GZipInputStream(File.OpenRead(inputPackageFile)))
            {
                using (var tarInputStream = new TarInputStream(inputStream))
                {
                    tarInputStream.IsStreamOwner = true;
                    foreach (var item in tarInputStream.Select())
                    {
                        if (!item.IsDirectory && item.Name.EndsWith("/pathname"))
                        {
                            var memoryStream = new MemoryStream((int)item.Size);
                            tarInputStream.CopyEntryContents(memoryStream);
                            memoryStream.Flush();
                            memoryStream.Position = 0;
                            using (var streamReader = new StreamReader(memoryStream))
                            {
                                var path = streamReader.ReadLine();
                                yield return path;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read the names of all the files contained in a .unitypackage file.
        /// </summary>
        /// <param name="inputPackageFile">The .unitypackage file to read</param>
        /// <returns>A hierarchical tree of file paths.</returns>
        public static FileTree UnityPackageTree(string inputPackageFile)
        {
            var fileTree = new FileTree("Assets");
            foreach (var file in UnityPackageFiles(inputPackageFile))
            {
                fileTree.AddPath(file, false);
            }
            return fileTree;
        }
    }
}