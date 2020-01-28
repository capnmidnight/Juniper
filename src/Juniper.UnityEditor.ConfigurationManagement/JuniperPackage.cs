using System;
using System.Collections.Generic;
using System.IO;

using Juniper.Compression;
using Juniper.Compression.Zip;

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

            var juniperPackagesRoot = Path.Combine(Project.JuniperAssetPath, "ThirdParty", "Optional");
            var rootDir = new DirectoryInfo(juniperPackagesRoot);
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
    }
}
