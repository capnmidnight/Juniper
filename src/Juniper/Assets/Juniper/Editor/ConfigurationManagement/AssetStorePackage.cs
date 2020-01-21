using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.Compression;
using Juniper.Compression.Tar.GZip;
using Juniper.IO;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    internal sealed class AssetStorePackage : AbstractFilePackage
    {
        private static readonly string USER_PROFILE = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static readonly string UNITY_ASSET_STORE_PATH = Path.Combine(USER_PROFILE, "AppData", "Roaming", "Unity", "Asset Store-5.x");
        private static readonly string USER_ASSET_STORE_PATH = Path.Combine(USER_PROFILE, "Projects", "Packages");
        private static readonly string[] ROOT_DIRECTORIES = new string[]
        {
            UNITY_ASSET_STORE_PATH,
            USER_ASSET_STORE_PATH
        };

        public static IEnumerable<AssetStorePackage> GetPackages(
            Dictionary<string, AbstractFilePackage> currentPackages,
            Dictionary<string, string> defines)
        {
            var packages = Decompressor.FindUnityPackages(ROOT_DIRECTORIES);
            return from tup in FilterPackages<AssetStorePackage>(packages, currentPackages)
                   select tup.pkg ?? new AssetStorePackage(tup.file, defines);
        }

        public AssetStorePackage(FileInfo file, Dictionary<string, string> defines)
            : base(new DirectoryInfo(Directory.GetCurrentDirectory()), file, defines)
        { }

        protected override IEnumerable<CompressedFileInfo> GetPackageFiles()
        {
            return Decompressor.UnityPackageEntries(FileName);
        }

        protected override void InstallInternal(IProgress prog)
        {
            if (File.Exists(FileName))
            {
                AssetDatabase.importPackageCompleted += AssetDatabase_importPackageCompleted;
                AssetDatabase.ImportPackage(FileName, true);
            }
        }

        private void AssetDatabase_importPackageCompleted(string packageName)
        {
            AssetDatabase.importPackageCompleted -= AssetDatabase_importPackageCompleted;
            InstallComplete();
        }
    }
}
