using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Juniper.Compression.Tar.GZip;
using Juniper.Progress;

using UnityEditor;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    internal sealed class AssetStorePackage : AbstractPackage, ISerializable
    {
        public enum Status
        {
            None,
            Found,
            NotFound,
            List,
            Listing,
            Listed,
            Scan,
            Scanning,
            Scanned,
            Error
        }

        private readonly FileInfo packageFile;
        private int installedFiles;
        private string[] paths;

        public event Action ScanningProgressUpdated;

        public AssetStorePackage(FileInfo file)
        {
            packageFile = file;
            Name = Path.GetFileNameWithoutExtension(packageFile.Name);
            ScanningProgress = Status.None;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("path", packageFile.FullName);
            info.AddValue(nameof(installedFiles), installedFiles);
            info.AddValue(nameof(progress), progress.ToString());
            info.AddValue(nameof(paths), paths);
        }

        public AssetStorePackage(SerializationInfo info, StreamingContext context)
        {
            packageFile = new FileInfo(info.GetString("path"));
            installedFiles = info.GetInt32(nameof(installedFiles));
            Enum.TryParse(info.GetString(nameof(progress)), out progress);
            paths = (string[])info.GetValue(nameof(paths), typeof(string[]));
        }

        public string InputUnityPackageFile
        {
            get
            {
                return packageFile.FullName;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return packageFile.LastWriteTime;
            }
        }

        public override bool IsInstalled
        {
            get
            {
                return ScanningProgress == Status.Scanned
                    && paths != null
                    && installedFiles >= paths.Length;
            }
        }

        private Status progress;
        public Status ScanningProgress
        {
            get
            {
                return progress;
            }
            private set
            {
                progress = value;
                OnScanningProgressUpdated();
            }
        }

        public string TotalFiles
        {
            get
            {
                if (paths == null)
                {
                    return "N/A";
                }
                else
                {
                    return paths.Length.ToString();
                }
            }
        }

        private void OnScanningProgressUpdated()
        {
            ScanningProgressUpdated?.Invoke();
        }

        public float InstallPercentage
        {
            get
            {
                if (paths == null)
                {
                    return 0;
                }
                else
                {
                    return (float)installedFiles / paths.Length;
                }
            }
        }

        public string ErrorMessage { get; private set; }

        public void Update()
        {
            try
            {
                if (ScanningProgress == Status.None)
                {
                    if (packageFile.Exists)
                    {
                        ScanningProgress = Status.Found;
                    }
                    else
                    {
                        ScanningProgress = Status.NotFound;
                    }
                }
                else if (ScanningProgress == Status.Found)
                {
                    ScanningProgress = Status.List;
                }
                else if (ScanningProgress == Status.List)
                {
                    ScanningProgress = Status.Listing;
                    Task.Run(List);
                }
                else if(ScanningProgress == Status.Listed)
                {
                    ScanningProgress = Status.Scan;
                }
                else if (ScanningProgress == Status.Scan)
                {
                    ScanningProgress = Status.Scanning;
                    Task.Run(Scan);
                }
            }
            catch(Exception exp)
            {
                ErrorMessage = exp.Message;
                ScanningProgress = Status.Error;
            }
        }

        private void List()
        {
            paths = Decompressor.UnityPackageFiles(InputUnityPackageFile).ToArray();
            installedFiles = 0;
            ScanningProgress = Status.Listed;
        }

        private void Scan()
        {
            foreach (var path in paths)
            {
                if (File.Exists(path) || Directory.Exists(path))
                {
                    ++installedFiles;
                }
            }
            ScanningProgress = Status.Scanned;
        }

        public override void Install(IProgress prog = null)
        {
            base.Install(prog);

            if (File.Exists(InputUnityPackageFile))
            {
                AssetDatabase.ImportPackage(InputUnityPackageFile, true);
                AssetDatabase.importPackageCompleted += AssetDatabase_importPackageCompleted;
            }

            prog?.Report(1);

        }

        private void AssetDatabase_importPackageCompleted(string packageName)
        {
            AssetDatabase.importPackageCompleted -= AssetDatabase_importPackageCompleted;
            ScanningProgress = Status.None;
        }

        private static void DeleteAll(IEnumerable<string> paths, Func<string, bool> tryDelete, IProgress prog)
        {
            prog?.Report(0, "Deleting");

            var prefixedPath = paths.ToArray();
            var subProgs = prog.Split(prefixedPath.Length);
            var index = 0;
            foreach (var path in prefixedPath)
            {
                try
                {
                    subProgs[index]?.Report(0);
                    if (tryDelete(path))
                    {
                        subProgs[index]?.Report(1);
                    }
                    ++index;

                    prog?.Report(index, prefixedPath.Length);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                }
            }

            prog?.Report(1, "Deleted");
        }

        public override void Uninstall(IProgress prog = null)
        {
            base.Uninstall(prog);

            var progs = prog.Split(3);

            var paths = Decompressor.UnityPackageFiles(InputUnityPackageFile);

            var files = paths.Where(File.Exists);
            files = files.Union(from file in files
                                where Path.GetExtension(file) != ".meta"
                                select file + ".meta");

            DeleteAll(files, FileExt.TryDelete, progs[0]);

            var dirs = from path in paths
                       where Directory.Exists(path)
                       orderby path.Length descending
                       select path;

            files = from dir in dirs
                    let exts = Directory.GetFiles(dir).Select(Path.GetExtension)
                    where exts.Count(ext => ext != ".meta") == 0
                    select dir + ".meta";

            DeleteAll(files, FileExt.TryDelete, progs[1]);
            DeleteAll(dirs, DirectoryExt.TryDelete, progs[2]);

            paths = null;
            installedFiles = 0;
            ScanningProgress = Status.None;
        }
    }
}
