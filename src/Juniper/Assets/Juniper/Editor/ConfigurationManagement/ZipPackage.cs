using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.Compression;
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
            Dictionary<string, string> defines)
        {
            var packages = Directory.GetFiles(ROOT_DIRECTORY, "*.zip");
            return from tup in FilterPackages<ZipPackage>(packages, currentPackages)
                   select tup.pkg ?? new ZipPackage(tup.file, defines);
        }

        public ZipPackage(
            FileInfo file,
            Dictionary<string, string> defines)
            : base(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets")), file, defines)
        { }


        protected override IEnumerable<CompressedFileInfo> GetPackageFiles()
        {
            return Decompressor.Entries(PackageFile);
        }


        protected override void InstallInternal(IProgress prog)
        {
            if (IsAvailable)
            {
                Decompressor.Decompress(FileName, installDirectory, prog);
                InstallComplete();
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
                PlayerSettings.vuforiaEnabled = true;
            }

            if (targetGroup == BuildTargetGroup.Android)
            {
                if (Name == "GoogleARCore")
                {
                    PlayerSettings.Android.ARCoreEnabled = true;
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
                    Juniper.ApplleiOS.RequiresARKitSupport = true;

                    if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    {
                        PlayerSettings.iOS.cameraUsageDescription = "Augmented reality camera view";
                    }
                }
            }
        }
    }
}
