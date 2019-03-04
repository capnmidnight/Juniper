using ICSharpCode.SharpZipLib.Zip;

using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juniper.Data
{
    public static class Zip
    {
        static Zip()
        {
            // Force SharpZipLib to use any code page that is available on the runtime system, rather
            // than stubbornly insisting on trying to use IBM437, which is only available by default
            // on Windows.
            ZipConstants.DefaultCodePage = 0;
        }

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
                    reject(new FileNotFoundException("Could not find file " + filePath + " in zip file " + zipFile, zipFile + "::" + filePath));
                }
            }
            prog?.SetProgress(1);
        }

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