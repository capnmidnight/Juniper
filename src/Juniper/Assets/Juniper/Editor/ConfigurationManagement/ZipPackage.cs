using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.Compression.Zip;
using Juniper.Progress;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    internal sealed class ZipPackage : AbstractFilePackage
    {
        public static string ROOT_DIRECTORY = PathExt.FixPath("Assets/Juniper/ThirdParty/Optional");

        public static IEnumerable<ZipPackage> GetPackages(
            Dictionary<string, AbstractFilePackage> currentPackages,
            Dictionary<string, string> defines,
            Dictionary<string, PackageInstallProgress> progresses)
        {
            var packages = Directory.GetFiles(ROOT_DIRECTORY, "*.zip");
            return from tup in FilterPackages<ZipPackage>(packages, currentPackages)
                   select tup.pkg ?? new ZipPackage(tup.file, defines, progresses);
        }

        public ZipPackage(
            FileInfo file,
            Dictionary<string, string> defines,
            Dictionary<string, PackageInstallProgress> progresses)
            : base(file, defines, progresses)
        { }

        protected override string[] GetPackageFileNames()
        {
            return Decompressor.FileNames(PackageFile).ToArray();
        }


        protected override void InstallInternal(IProgress prog)
        {
            if (IsAvailable)
            {
                Decompressor.Decompress(FileName, "Assets", prog);
                ImportComplete();
            }
        }

        private bool IsAvailable
        {
            get
            {
                return File.Exists(FileName);
            }
        }

        public override void Activate(BuildTargetGroup targetGroup, IProgress prog)
        {
            base.Activate(targetGroup, prog);

            if (Name == "Vuforia")
            {
#if UNITY_2019_2_OR_NEWER
                PlayerSettings.vuforiaEnabled = true;
#else
                PlayerSettings.SetPlatformVuforiaEnabled(targetGroup, true);
#endif
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
    }
}
