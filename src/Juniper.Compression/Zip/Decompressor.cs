using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;

using Juniper.Progress;

namespace Juniper.Compression.Zip
{
    /// <summary>
    /// Functions for dealing with Zip files.
    /// </summary>
    public static class Decompressor
    {
        ///// <summary>
        ///// Force SharpZipLib to use any code page that is available on the runtime system, rather
        ///// than stubbornly insisting on trying to use IBM437, which is only available by default
        ///// on Windows.
        ///// </summary>
        static Decompressor()
        {
            ZipStrings.UseUnicode = true;
        }

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="zipFile">The zip file in which to find the file.</param>
        /// <param name="filePath">The file to find in the zip file.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        public static Stream GetFile(string zipFile, string filePath, IProgress prog = null)
        {
            prog?.Report(0);
            using (var file = new ZipFile(zipFile))
            {
                file.IsStreamOwner = true;
                var entryIndex = file.FindEntry(filePath, true);
                if (entryIndex > -1)
                {
                    var entry = file[entryIndex];
                    using (var fileStream = file.GetInputStream(entry))
                    using (var progStream = new ProgressStream(fileStream, entry.Size, prog))
                    {
                        var mem = new MemoryStream();
                        progStream.CopyTo(mem);
                        mem.Flush();
                        mem.Position = 0;
                        file.Close();
                        return mem;
                    }
                }
                else
                {
                    file.Close();
                    throw new FileNotFoundException($"Could not find file {filePath} in zip file {zipFile}", $"{zipFile}::{filePath}");
                }
            }
        }

        /// <summary>
        /// Enumerates all of the entires in a single Zip archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static IEnumerable<T> Select<T>(this ZipInputStream zip, Func<ZipEntry, ZipInputStream, bool> checkEntry, Func<ZipEntry, ZipInputStream, T> eachEntry)
        {
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null)
            {
                if (checkEntry(entry, zip))
                {
                    yield return eachEntry(entry, zip);
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Zip file.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the Zip file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        private static IEnumerable<T> Select<T>(string inputZipFile, Func<ZipEntry, ZipInputStream, bool> checkEntry, Func<ZipEntry, ZipInputStream, T> eachEntry)
        {
            if (!File.Exists(inputZipFile))
            {
                throw new FileNotFoundException("File not found! " + inputZipFile, inputZipFile);
            }
            else
            {
                using (var zip = new ZipInputStream(File.OpenRead(inputZipFile)))
                {
                    zip.IsStreamOwner = true;
                    return zip.Select(checkEntry, eachEntry);
                }
            }
        }

        /// <summary>
        /// Enumerates all of the entires in a single Zip archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of ZipEntry objects, as filtered by <paramref name="checkEntry"/>.</returns>
        public static IEnumerable<ZipEntry> Select(this ZipInputStream zip, Func<ZipEntry, ZipInputStream, bool> checkEntry)
        {
            return zip.Select(
                checkEntry,
                (entry, _) => entry);
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Zip file.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the Zip file to scan.</param>
        /// <param name="checkEntry">A callback to check if a particular file should be processed</param>
        /// <returns>A lazy collection of ZipEntries</returns>
        public static IEnumerable<ZipEntry> Select(string inputZipFile, Func<ZipEntry, ZipInputStream, bool> checkEntry)
        {
            return Select(
                inputZipFile,
                checkEntry,
                (entry, _) => entry);
        }

        /// <summary>
        /// Enumerates all of the entires in a single Zip archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip"></param>
        /// <returns>A lazy collection of ZipEntry objects.</returns>
        public static IEnumerable<ZipEntry> Select(this ZipInputStream zip)
        {
            return zip.Select(
                (_, __) => true,
                (entry, _) => entry);
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a Zip file.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the Zip file to scan.</param>
        /// <returns>A lazy collection of ZipEntries</returns>
        public static IEnumerable<ZipEntry> Select(string inputZipFile)
        {
            return Select(
                inputZipFile,
                (_, __) => true,
                (entry, _) => entry);
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static void ForEach(this ZipInputStream zip, Action<ZipEntry, ZipInputStream> eachEntry, Action<Exception> error = null)
        {
            ZipEntry entry;
            while ((entry = zip.GetNextEntry()) != null)
            {
                try
                {
                    eachEntry(entry, zip);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    error?.Invoke(exp);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>
        /// Enumerates all of the entires in a single TAR archive so that it can be used
        /// more easily with c#'s for-each.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the Zip file to scan.</param>
        /// <param name="eachEntry">A callback to process each entry that passes <paramref name="checkEntry"/>.</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        /// <returns>A lazy collection of <typeparamref name="T"/> objects, as filtered by <paramref name="checkEntry"/>, as selected by <paramref name="eachEntry"/>.</returns>
        public static void ForEach(string inputZipFile, Action<ZipEntry, ZipInputStream> eachEntry, Action<Exception> error = null)
        {
            if (!File.Exists(inputZipFile))
            {
                throw new FileNotFoundException("File not found! " + inputZipFile, inputZipFile);
            }
            else
            {
                using (var zip = new ZipInputStream(File.OpenRead(inputZipFile)))
                {
                    zip.IsStreamOwner = true;
                    ZipEntry entry;
                    while ((entry = zip.GetNextEntry()) != null)
                    {
                        try
                        {
                            eachEntry(entry, zip);
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
        /// Dump the contents of a zip file out to disk.
        /// </summary>
        /// <param name="inputZipFile">The zip file to decompress</param>
        /// <param name="outputDirectory">The location to which to dump the files.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        public static void Decompress(string inputZipFile, string outputDirectory, IProgress prog = null, Action<Exception> error = null)
        {
            prog?.Report(0);
            if (!File.Exists(inputZipFile))
            {
                throw new FileNotFoundException("File not found! " + inputZipFile, inputZipFile);
            }
            else
            {
                ForEach(inputZipFile, (zipEntry, zipStream) =>
                {
                    var fullZipToPath = Path.Combine(outputDirectory, zipEntry.Name);
                    if (zipEntry.IsDirectory)
                    {
                        DirectoryExt.CreateDirectory(fullZipToPath);
                    }
                    else if (zipEntry.IsFile
                        && (!File.Exists(fullZipToPath)
                            || FileExt.TryDelete(fullZipToPath)))
                    {
                        var directoryName = Path.GetDirectoryName(fullZipToPath);
                        if (directoryName.Length > 0)
                        {
                            DirectoryExt.CreateDirectory(directoryName);
                        }

                        zipStream.CopyTo(File.Create(fullZipToPath));
                    }
                }, error);
            }
        }

        /// <summary>
        /// Get the names of all the directories in the zip file.
        /// </summary>
        /// <param name="inputZipFile"></param>
        /// <returns></returns>
        public static IEnumerable<string> DirectoryNames(string inputZipFile)
        {
            return Select(inputZipFile,
                (entry, _) => entry.IsDirectory,
                (entry, _) => entry.Name);
        }

        /// <summary>
        /// Get the names of all the files in the zip file.
        /// </summary>
        /// <param name="inputZipFile"></param>
        /// <returns></returns>
        public static IEnumerable<string> FileNames(string inputZipFile)
        {
            return Select(inputZipFile,
                (entry, _) => entry.IsFile,
                (entry, _) => entry.Name);
        }
    }
}
