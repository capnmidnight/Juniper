using ICSharpCode.SharpZipLib.Zip;

using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juniper.Zip
{
    /// <summary>
    /// Functions for dealing with Zip files.
    /// </summary>
    public static class Compressor
    {
        ///// <summary>
        ///// Force SharpZipLib to use any code page that is available on the runtime system, rather
        ///// than stubbornly insisting on trying to use IBM437, which is only available by default
        ///// on Windows.
        ///// </summary>
        static Compressor()
        {
            ZipStrings.UseUnicode = true;
        }

        /// <summary>
        /// Makes a zip file out of the contents of a directory.
        /// </summary>
        /// <param name="inputDirectory">The directory to zip</param>
        /// <param name="outputZipFile">The filepath of the zip file to create</param>
        /// <param name="level">The zip compression level to use. Min is 0, max is 9.</param>
        /// <param name="prog">A progress tracking object, defaults to null (i.e. no progress tracking).</param>
        /// <param name="error">A callback for any errors that occur. Defaults to null (i.e. no error reporting).</param>
        public static void CompressDirectory(string inputDirectory, string outputZipFile, int level, IProgress prog = null, Action<Exception> error = null)
        {
            prog?.Report(0);

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
                        using (var streamReader = new ProgressStream(fi.OpenRead(), fi.Length, p))
                        {
                            streamReader.CopyTo(zipStream);
                        }
                        zipStream.CloseEntry();
                    }, error);
                }
            }

            prog?.Report(1);
        }
    }
}
