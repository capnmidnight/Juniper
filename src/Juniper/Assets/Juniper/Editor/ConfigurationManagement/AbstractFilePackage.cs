using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Progress;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{

    internal abstract class AbstractFilePackage : AbstractPackage
    {
        protected static IEnumerable<(FileInfo file, T pkg)> FilterPackages<T>(
            IEnumerable<string> packages,
            Dictionary<string, AbstractFilePackage> currentPackages)
            where T : AbstractFilePackage
        {
            return from path in packages
                   let file = new FileInfo(path)
                   let name = Path.GetFileNameWithoutExtension(file.Name)
                   let curPkg = currentPackages.Get(name) as T
                   select (file, curPkg);
        }

        private static void DeleteAll(IEnumerable<string> paths, Func<string, bool> tryDelete, IProgress prog)
        {
            prog?.Report(0, "Deleting");

            var prefixedPath = paths
                .Select(path => Path.Combine("Assets", path))
                .ToArray();

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

        public readonly FileInfo PackageFile;
        private readonly PackageInstallProgress Progress;

        public event Action ScanningProgressUpdated;

        public bool Changed { get; private set; }

        public void Save() { Changed = false; }

        public override string CompilerDefine
        {
            get
            {
                return base.CompilerDefine;
            }

            set
            {
                if(base.CompilerDefine != value)
                {
                    Changed = true;
                    base.CompilerDefine = value;
                }
            }
        }

        public AbstractFilePackage(FileInfo file, Dictionary<string, string> defines, Dictionary<string, PackageInstallProgress> progresses)
        {
            PackageFile = file;
            Name = Path.GetFileNameWithoutExtension(PackageFile.Name);
            CompilerDefine = defines.Get(Name, Name.Replace(" ", "_").Replace(".", "_").ToUpperInvariant());
            Progress = progresses.Get(PackageFile.FullName, new PackageInstallProgress(PackageFile));
            ScanningProgress = PackageScanStatus.None;
            GUILabel = new GUIContent(Name, Name);
        }

        public PackageInstallProgress GetProgress()
        {
            return Progress;
        }

        public string FileName
        {
            get
            {
                return PackageFile.FullName;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return PackageFile.LastWriteTime;
            }
        }

        public override bool IsInstalled
        {
            get
            {
                return ScanningProgress == PackageScanStatus.Scanned
                    && Progress != null
                    && Paths != null
                    && Paths.Length > 0
                    && InstalledFiles / Paths.Length >= 0.1f;
            }
        }

        public PackageScanStatus ScanningProgress
        {
            get
            {
                return Progress.progress;
            }
            private set
            {
                if (ScanningProgress != value)
                {
                    Changed = true;
                    Progress.progress = value;
                    OnScanningProgressUpdated();
                }
            }
        }

        public string[] Paths
        {
            get
            {
                return Progress.paths;
            }
            private set
            {
                if (!Paths.Matches(value))
                {
                    Changed = true;
                    Progress.paths = value;
                    InstalledFiles = 0;

                    if (value != null)
                    {
                        ScanningProgress = PackageScanStatus.Listed;
                    }
                    else
                    {
                        ScanningProgress = PackageScanStatus.None;
                    }
                }
            }
        }

        public int InstalledFiles
        {
            get
            {
                return Progress.installedFiles;
            }
            private set
            {
                if(InstalledFiles != value)
                {
                    Changed = true;
                    InstalledFiles = value;
                }
            }
        }

        protected void ImportComplete()
        {
            ScanningProgress = PackageScanStatus.None;
        }

        public string TotalFiles
        {
            get
            {
                if (Paths == null)
                {
                    return "N/A";
                }
                else
                {
                    return Paths.Length.ToString();
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
                if (Paths == null)
                {
                    return 0;
                }
                else
                {
                    return (float)InstalledFiles / Paths.Length;
                }
            }
        }

        public string ErrorMessage { get; private set; }

        public GUIContent GUILabel { get; internal set; }

        public void Update()
        {
            try
            {
                if (ScanningProgress == PackageScanStatus.None)
                {
                    if (PackageFile.Exists)
                    {
                        ScanningProgress = PackageScanStatus.Found;
                    }
                    else
                    {
                        ScanningProgress = PackageScanStatus.NotFound;
                    }
                }
                else if (ScanningProgress == PackageScanStatus.Found)
                {
                    ScanningProgress = PackageScanStatus.List;
                }
                else if (ScanningProgress == PackageScanStatus.List)
                {
                    ScanningProgress = PackageScanStatus.Listing;
                    Task.Run(List);
                }
                else if (ScanningProgress == PackageScanStatus.Listed)
                {
                    ScanningProgress = PackageScanStatus.Scan;
                }
                else if (ScanningProgress == PackageScanStatus.Scan)
                {
                    ScanningProgress = PackageScanStatus.Scanning;
                    Task.Run(Scan);
                }
            }
            catch (Exception exp)
            {
                ErrorMessage = exp.Message;
                ScanningProgress = PackageScanStatus.Error;
            }
        }

        private void List()
        {
            Paths = GetPackageFileNames();
        }

        private void Scan()
        {
            foreach (var path in Paths)
            {
                if (File.Exists(path) || Directory.Exists(path))
                {
                    ++InstalledFiles;
                }
            }
            ScanningProgress = PackageScanStatus.Scanned;
        }

        public override void Uninstall(IProgress prog)
        {
            base.Uninstall(prog);

            var progs = prog.Split(3);
            var paths = GetPackageFileNames();
            var files = paths.Where(File.Exists);
            var dirs = (from path in files
                        orderby path.Length descending
                        select Path.GetDirectoryName(path))
                    .Distinct();

            files = files.Union(from file in files
                                where Path.GetExtension(file) != ".meta"
                                select file + ".meta");

            DeleteAll(files, FileExt.TryDelete, progs[0]);

            files = from dir in dirs
                    let exts = Directory.GetFiles(dir).Select(Path.GetExtension)
                    where exts.Count(ext => ext != ".meta") == 0
                    select dir + ".meta";

            DeleteAll(files, FileExt.TryDelete, progs[1]);
            DeleteAll(dirs, DirectoryExt.TryDelete, progs[2]);

            Paths = null;
        }
        protected abstract string[] GetPackageFileNames();
    }
}
