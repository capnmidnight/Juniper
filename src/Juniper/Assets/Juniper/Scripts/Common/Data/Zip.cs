using ICSharpCode.SharpZipLib.Zip;

using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juniper.Data
{
    /// <summary>
    /// Functions for dealing with Zip files.
    /// </summary>
    public static class Zip
    {
        /// <summary>
        /// Force SharpZipLib to use any code page that is available on the runtime system, rather
        /// than stubbornly insisting on trying to use IBM437, which is only available by default
        /// on Windows.
        /// </summary>
        static Zip()
        {
            ZipConstants.DefaultCodePage = 0;
        }

        /// <summary>
        /// Retrieves a single file from a zip file.
        /// </summary>
        /// <param name="zipFile">The zip file in which to find the file.</param>
        /// <param name="filePath">The file to find in the zip file.</param>
        /// <param name="resolve">A callback to recieve the file stream for the requested file.</param>
        /// <param name="reject">A callback if the file does not exist.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        public static void GetFile(string zipFile, string filePath, Action<Stream> resolve, Action<Exception> reject, IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);
            using (var file = new ZipFile(zipFile))
            {
                file.IsStreamOwner = true;
                var entryIndex = file.FindEntry(filePath, true);
                if (entryIndex > -1)
                {
                    var entry = file[entryIndex];
                    using (var stream = file.GetInputStream(entry))
                    {
                        resolve(new StreamProgress(stream, prog));
                    }
                }
                else
                {
                    reject(new FileNotFoundException($"Could not find file {filePath} in zip file {zipFile}", $"{zipFile}::{filePath}"));
                }
            }
            prog?.SetProgress(1);
        }

        /// <summary>
        /// Makes a zip file out of the contents of a directory.
        /// </summary>
        /// <param name="inputDirectory">The directory to zip</param>
        /// <param name="outputZipFile">The filepath of the zip file to create</param>
        /// <param name="level">The zip compression level to use. Min is 0, max is 9.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        public static void CompressDirectory(string inputDirectory, string outputZipFile, int level, IProgressReceiver prog = null, Action<Exception> error = null)
        {
            prog?.SetProgress(0);

            if (Directory.Exists(inputDirectory))
            {
                using (var fileStream = File.Create(outputZipFile))
                using (var zipStream = new ZipOutputStream(fileStream))
                {
                    zipStream.IsStreamOwner = false;
                    // To permit the zip to be unpacked by built-in extractor in WinXP and
                    // Server2003, WinZip 8, Java, and other older code, you need to do one of the
                    // following: Specify UseZip64.Off, or set the Size. If the file may be bigger
                    // than 4GB, or you do not need WinXP built-in compatibility, you do not need
                    // either, but the zip will be in Zip64 format which not all utilities can understand.
                    zipStream.UseZip64 = UseZip64.Off;

                    zipStream.SetLevel(Math.Max(0, Math.Min(9, level)));

                    var baseDirectory = new DirectoryInfo(inputDirectory);
                    var files = baseDirectory.RecurseFiles().ToArray();
                    prog.ForEach(files, (fi, p) =>
                    {
                        var shortName = PathExt.Abs2Rel(fi.FullName, baseDirectory.FullName);
                        var entryName = ZipEntry.CleanName(shortName);
                        var newEntry = new ZipEntry(entryName)
                        {
                            DateTime = fi.LastWriteTime,
                            Size = fi.Length
                        };

                        zipStream.PutNextEntry(newEntry);
                        using (var streamReader = fi.OpenRead())
                        {
                            streamReader.Pipe(zipStream, p);
                        }
                        zipStream.CloseEntry();
                    }, error);
                }
            }

            prog?.SetProgress(1);
        }

        /// <summary>
        /// Dump the contents of a zip file out to disk.
        /// </summary>
        /// <param name="inputZipFile">The zip file to decompress</param>
        /// <param name="outputDirectory">The location to which to dump the files.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        public static void DecompressDirectory(string inputZipFile, string outputDirectory, IProgressReceiver prog = null, Action<Exception> error = null)
        {
            prog?.SetProgress(0);
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
                        else if (zipEntry.IsFile)
                        {
                            var okToWrite = !File.Exists(fullZipToPath);
                            if (!okToWrite)
                            {
                                okToWrite = FileExt.TryDelete(fullZipToPath);
                            }

                            if (okToWrite)
                            {
                                var directoryName = Path.GetDirectoryName(fullZipToPath);
                                if (directoryName.Length > 0)
                                {
                                    DirectoryExt.CreateDirectory(directoryName);
                                }

                                using (var zipStream = zf.GetInputStream(zipEntry))
                                using (var streamWriter = File.Create(fullZipToPath))
                                {
                                    zipStream.Pipe(streamWriter, p);
                                }
                            }
                        }
                    }, error);
                }
            }
        }

        /// <summary>
        /// Lists all of the entries (both files and directories) in a zip file.
        /// </summary>
        /// <param name="inputZipFile">A filepath to the zip file to scan.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of ZipEntries</returns>
        public static IEnumerable<ZipEntry> ZipEntries(string inputZipFile, IProgressReceiver prog = null)
        {
            prog?.SetProgress(0);

            if (File.Exists(inputZipFile))
            {
                using (var zf = new ZipFile(inputZipFile))
                {
                    zf.IsStreamOwner = true;
                    var progs = prog.Split(zf.Count);
                    for (var i = 0; i < zf.Count; ++i)
                    {
                        var p = progs[i];
                        p?.SetProgress(0);

                        yield return zf[i];
                    }
                }
            }

            prog?.SetProgress(1);
        }

        /// <summary>
        /// Lists all of the files in a zip file.
        /// </summary>
        /// <param name="inputZipFile">A filepath to the zip file to scan.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of ZipEntries that are files.</returns>
        public static IEnumerable<string> RecurseFiles(string inputZipFile, IProgressReceiver prog = null)
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
        /// <param name="inputZipFile">A filepath to the zip file to scan.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <returns>A lazy collection of ZipEntries that are directories.</returns>
        public static IEnumerable<string> RecurseDirectories(string inputZipFile, IProgressReceiver prog = null)
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
