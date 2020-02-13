using System;
using System.Collections.Generic;
using System.IO;

using Juniper.Compression;
using Juniper.Compression.Zip;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    public sealed class JuniperPackage : AbstractCompressedPackage
    {
        public static void Load(List<AbstractPackage> packages)
        {
            if (packages is null)
            {
                throw new ArgumentNullException(nameof(packages));
            }

            var rootDir = new DirectoryInfo(Project.JuniperAssetStoreCachePath);
            foreach (var file in rootDir.GetFiles("*.zip"))
            {
                var packageName = Path.GetFileNameWithoutExtension(file.Name);
                var packagePath = file.FullName;
                packages.Add(new JuniperPackage(packageName, null, packagePath));
            }
        }

        public JuniperPackage(string name, string version, string path)
            : base(PackageSources.Juniper, name, version, path)
        { }

        protected override string InstallDirectory => Project.UnityAssetsPath;

        protected override IEnumerable<CompressedFileInfo> GetContentFiles()
        {
            return Decompressor.Entries(ContentPath);
        }

        public override void Install()
        {
            Decompressor.Decompress(ContentPath, InstallDirectory);
        }

        public override void Activate()
        {
            var targetGroup = Project.CurrentBuildTargetGroup;

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
                    ApplleiOS.RequiresARKitSupport = true;

                    if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    {
                        PlayerSettings.iOS.cameraUsageDescription = "Augmented reality camera view";
                    }
                }
            }
        }
    }
}
