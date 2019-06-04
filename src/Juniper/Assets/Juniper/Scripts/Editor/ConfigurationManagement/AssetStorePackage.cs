using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.Compression.Tar.GZip;
using Juniper.Progress;

using Newtonsoft.Json;

using UnityEditor;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    internal sealed class AssetStorePackage : AbstractPackage
    {
        private readonly FileInfo packageFile;
        private bool installed;

        public AssetStorePackage(FileInfo file)
        {
            packageFile = file;
            Name = Path.GetFileNameWithoutExtension(packageFile.Name);
        }

        [JsonIgnore]
        public string InputUnityPackageFile
        {
            get
            {
                return packageFile.FullName;
            }
        }

        [JsonIgnore]
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
                return installed;
            }
        }

        public float InstallPercentage { get; private set; }

        public void UpdateStats()
        {
            installed = true;
            float n = 0, m = 0;
            foreach (var file in Decompressor.UnityPackageFiles(InputUnityPackageFile))
            {
                ++m;
                if (File.Exists(file))
                {
                    ++n;
                }
                else
                {
                    installed = false;
                }
            }

            InstallPercentage = n / m;
        }

        public override void Install(IProgress prog = null)
        {
            base.Install(prog);

            if (File.Exists(InputUnityPackageFile))
            {
                AssetDatabase.ImportPackage(InputUnityPackageFile, true);
            }

            prog?.Report(1);
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

        public override void Uninstall(IProgress prog = null)
        {
            base.Uninstall(prog);

            var progs = prog.Split(4);

            var files = Decompressor.UnityPackageFiles(InputUnityPackageFile);
            var dirs = (from file in files
                        select Path.GetDirectoryName(file))
                    .Distinct();
            files = files
                .Union(files.Select(file => file + ".meta"))
                .Union(dirs.Select(dir => dir + ".meta"))
                .Where(path => !path.EndsWith(".meta.meta"));
            DeleteAll(files, FileExt.TryDelete, progs[1]);
            DeleteAll(dirs.Reverse(), DirectoryExt.TryDelete, progs[3]);
        }
    }
}
