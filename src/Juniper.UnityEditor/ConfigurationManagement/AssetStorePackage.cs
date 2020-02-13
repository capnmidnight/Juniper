using System;
using System.Collections.Generic;
using System.IO;

using Juniper.Compression;
using Juniper.Compression.Tar.GZip;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    public sealed class AssetStorePackage : AbstractCompressedPackage
    {
        public static void Load(List<AbstractPackage> packages)
        {
            if (packages is null)
            {
                throw new ArgumentNullException(nameof(packages));
            }

            AddPackagesFromDirectory(packages, Project.AssetStoreRoot);
            AddPackagesFromDirectory(packages, Project.UserAssetStoreRoot);
            AddPackagesFromDirectory(packages, Project.UserDownloads);
        }

        private static void AddPackagesFromDirectory(List<AbstractPackage> packages, string root)
        {
            var rootDir = new DirectoryInfo(root);
            if (rootDir.Exists)
            {
                foreach (var vendorDir in rootDir.GetDirectories())
                {
                    var vendor = vendorDir.Name;
                    foreach (var purposeDir in vendorDir.GetDirectories())
                    {
                        foreach (var packageFile in purposeDir.GetFiles("*.unitypackage"))
                        {
                            var packageName = $"{vendorDir.Name} - {Path.GetFileNameWithoutExtension(packageFile.Name)}";
                            var packagePath = packageFile.FullName;
                            packages.Add(new AssetStorePackage(packageName, null, packagePath));
                        }
                    }
                }
            }
        }

        public AssetStorePackage(string name, string version, string path)
            : base(PackageSources.UnityAssetStore, name, version, path)
        { }

        protected override string InstallDirectory => Project.UnityProjectRoot;

        protected override IEnumerable<CompressedFileInfo> GetContentFiles()
        {
            return Decompressor.UnityPackageEntries(ContentPath);
        }

        public override void Install()
        {
            if (File.Exists(ContentPath))
            {
                AssetDatabase.ImportPackage(ContentPath, true);
            }
        }
    }
}
