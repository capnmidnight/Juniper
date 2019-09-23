using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Juniper.Collections;
using Juniper.Compression;
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

        protected readonly DirectoryInfo installDirectory;
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
                if (base.CompilerDefine != value)
                {
                    Changed = true;
                    base.CompilerDefine = value;
                }
            }
        }

        public AbstractFilePackage(DirectoryInfo installDirectory, FileInfo file, Dictionary<string, string> defines)
        {
            this.installDirectory = installDirectory;
            PackageFile = file;
            Name = Path.GetFileNameWithoutExtension(PackageFile.Name);
            CompilerDefine = defines.Get(Name, Name.Replace(" ", "_").Replace(".", "_").ToUpperInvariant());
            Progress = new PackageInstallProgress(PackageFile);
            ScanningProgress = PackageScanStatus.None;
            GUILabel = new GUIContent(Name, Name);
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
                    && Paths.Count > 0
                    && InstalledFiles / Paths.Count >= 0.1f;
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

        public NAryTree<CompressedFileInfo> Paths
        {
            get
            {
                return Progress.tree;
            }
            private set
            {
                Changed = true;
                Progress.tree = value;

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

        public int InstalledFiles
        {
            get
            {
                return Progress.installedFiles;
            }
            private set
            {
                if (InstalledFiles != value)
                {
                    Changed = true;
                    Progress.installedFiles = value;
                }
            }
        }

        protected void InstallComplete()
        {
            ScanningProgress = PackageScanStatus.Scan;
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
                    return Paths.Count.ToString();
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
                    return (float)InstalledFiles / Paths.Count;
                }
            }
        }

        public Exception Error { get; private set; }

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
                Error = exp;
                ScanningProgress = PackageScanStatus.Error;
            }
        }

        private void List()
        {
            try
            {
                Paths = GetPackageTree();
            }
            catch (Exception exp)
            {
                Error = exp;
                ScanningProgress = PackageScanStatus.Error;
            }
        }

        internal void ClearError()
        {
            Error = null;
            ScanningProgress = PackageScanStatus.None;
        }

        private void Scan()
        {
            try
            {
                if (Paths == null)
                {
                    ScanningProgress = PackageScanStatus.List;
                }
                else
                {
                    InstalledFiles = Paths.Where(path =>
                    {
                        var fullPath = path.Value.Name == null
                            ? installDirectory.FullName
                            : Path.Combine(installDirectory.FullName, path.Value.Name);
                        return path.Value.IsDirectory && Directory.Exists(fullPath)
                            || path.Value.IsFile && File.Exists(fullPath);
                    }).Count() - 1;
                    ScanningProgress = PackageScanStatus.Scanned;
                }
            }
            catch (Exception exp)
            {
                Error = exp;
                ScanningProgress = PackageScanStatus.Error;
            }
        }

        protected override void InstallInternal(IProgress prog)
        {
            if (Paths == null)
            {
                List();
            }
        }

        public override void Uninstall(IProgress prog)
        {
            if (Paths == null)
            {
                List();
            }
            base.Uninstall(prog);
            var paths = Paths.Flatten(TreeTraversalOrder.DepthFirst)
                .Reverse()
                .ToArray();

            for (var i = 0; i < paths.Length; ++i)
            {
                try
                {
                    prog.Report(i, paths.Length);
                    var path = paths[i].Value;
                    if (path.Name != null)
                    {
                        var fullPath = Path.Combine(installDirectory.FullName, path.Name);
                        if (path.IsFile)
                        {
                            File.Delete(fullPath);
                        }
                        else if (path.IsDirectory)
                        {
                            Directory.Delete(fullPath);
                        }
                    }
                    prog.Report(i + 1, paths.Length);
                }
                catch { }
            }

            ScanningProgress = PackageScanStatus.Scan;
        }
        protected abstract NAryTree<CompressedFileInfo> GetPackageTree();
    }
}
