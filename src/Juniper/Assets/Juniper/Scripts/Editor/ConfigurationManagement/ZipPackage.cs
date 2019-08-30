using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.Compression.Zip;
using Juniper.Progress;

using Json.Lite;

using UnityEditor;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    internal sealed class ZipPackage : AbstractPackage
    {
        public static string ROOT_DIRECTORY = PathExt.FixPath("Assets/Juniper/ThirdParty/Optional");

        [JsonIgnore]
        public string InputZipFileName
        {
            get
            {
                return Path.ChangeExtension(Path.Combine(ROOT_DIRECTORY, Name), "zip");
            }
        }

        public override bool IsInstalled
        {
            get
            {
                foreach(var file in Decompressor.FileNames(InputZipFileName))
                {
                    var installPath = Path.Combine("Assets", file);
                    if(!File.Exists(installPath))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override void Install(IProgress prog = null)
        {
            base.Install(prog);

            if (File.Exists(InputZipFileName))
            {
                Decompressor.Decompress(InputZipFileName, "Assets", prog);
            }

            prog?.Report(1);
        }

        public override void Activate(BuildTargetGroup targetGroup, IProgress prog = null)
        {
            base.Activate(targetGroup, prog);

            if (Name == "Vuforia")
            {
                PlayerSettings.SetPlatformVuforiaEnabled(targetGroup, true);
            }

            if (targetGroup == BuildTargetGroup.Android)
            {
                if (Name == "GoogleARCore")
                {
#if UNITY_2018_2_OR_NEWER
                    PlayerSettings.Android.ARCoreEnabled = true;
#endif
                }
                else if (Name == "GoogleVR")
                {
                    FileExt.Copy(
                        PathExt.FixPath("Assets/GoogleVR/Plugins/Android/AndroidManifest-6DOF.xml"),
                        PathExt.FixPath("Assets/Plugins/Android/AndroidManifest.xml"),
                        true);
                }
            }
            else if (targetGroup == BuildTargetGroup.iOS)
            {
                if (Name == "UnityARKitPlugin")
                {
                    PlayerSettingsExt.iOS.RequiresARKitSupport = true;

                    if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    {
                        PlayerSettings.iOS.cameraUsageDescription = "Augmented reality camera view";
                    }
                }
            }
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

            var progs = prog.Split(3);

            var files = Decompressor.FileNames(InputZipFileName).Where(File.Exists);
            files = files.Union(from file in files
                                where Path.GetExtension(file) != ".meta"
                                select file + ".meta");

            DeleteAll(files, FileExt.TryDelete, progs[0]);

            var dirs = from dir in Decompressor.DirectoryNames(InputZipFileName)
                       where Directory.Exists(dir)
                       orderby dir.Length descending
                       select dir;

            files = from dir in dirs
                    let exts = Directory.GetFiles(dir).Select(Path.GetExtension)
                    where exts.Count(ext => ext != ".meta") == 0
                    select dir + ".meta";

            DeleteAll(files, FileExt.TryDelete, progs[1]);
            DeleteAll(dirs, DirectoryExt.TryDelete, progs[2]);
        }
    }
}
