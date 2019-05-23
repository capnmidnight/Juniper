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
                    return new ProgressStream(file.GetInputStream(entry), entry.Size, prog);
                }
                else
                {
                    throw new FileNotFoundException($"Could not find file {filePath} in zip file {zipFile}", $"{zipFile}::{filePath}");
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
        public static void DecompressDirectory(string inputZipFile, string outputDirectory, IProgress prog = null, Action<Exception> error = null)
        {
            prog?.Report(0);
            if (!File.Exists(inputZipFile))
            {
                throw new FileNotFoundException("File not found! " + inputZipFile, inputZipFile);
            }
            else
            {
                using (var zf = new ZipFile(inputZipFile))
                {
                    zf.IsStreamOwner = true;
                    prog.ForEach(zf.Cast<ZipEntry>(), (zipEntry, p) =>
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

                            using (var zipStream = zf.GetInputStream(zipEntry))
                            using (var streamWriter = new ProgressStream(File.Create(fullZipToPath), zipEntry.Size, p))
                            {
                                zipStream.CopyTo(streamWriter);
                            }
                        }
                    }, error);
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a zip file.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the zip file to scan.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of ZipEntries</returns>
        public static IEnumerable<ZipEntry> ZipEntries(string inputZipFile, IProgress prog = null)
        {
            prog?.Report(0);

            if (File.Exists(inputZipFile))
            {
                using (var zf = new ZipFile(inputZipFile))
                {
                    zf.IsStreamOwner = true;
                    var progs = prog.Split(zf.Count);
                    for (var i = 0; i < zf.Count; ++i)
                    {
                        var p = progs[i];
                        p?.Report(0);

                        yield return zf[i];
                    }
                }
            }

            prog?.Report(1);
        }

        /// <summary>
        /// Lists all of the files in a zip file.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the zip file to scan.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of ZipEntries that are files.</returns>
        public static IEnumerable<string> RecurseFiles(string inputZipFile, IProgress prog = null)
        {
            foreach (var zipEntry in ZipEntries(inputZipFile, prog))
            {
                if (zipEntry.IsFile)
                {
                    yield return PathExt.FixPath(zipEntry.Name);
                }
            }
        }

        /// <summary>
        /// Lists all of the directories in a zip file.
        /// </summary>
        /// <param name="inputZipFile">A file-path to the zip file to scan.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of ZipEntries that are directories.</returns>
        public static IEnumerable<string> RecurseDirectories(string inputZipFile, IProgress prog = null)
        {
            foreach (var zipEntry in ZipEntries(inputZipFile, prog))
            {
                if (zipEntry.IsDirectory)
                {
                    yield return PathExt.FixPath(zipEntry.Name);
                }
            }
        }
    }
}
