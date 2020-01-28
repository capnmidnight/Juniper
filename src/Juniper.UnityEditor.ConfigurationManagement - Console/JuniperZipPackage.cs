using System;
using System.Collections.Generic;
using System.IO;
using Juniper.Compression;
using Juniper.Compression.Zip;

namespace Juniper.ConfigurationManagement
{
    public class JuniperZipPackage : AbstractCompressedPackage
    {
        public static void GetPackages(List<AbstractPackage2> packages)
        {
            if (packages is null)
            {
                throw new ArgumentNullException(nameof(packages));
            }

            var juniperPath = Path.Combine(UnityProjectRoot, "Assets", "Juniper");
            var juniperThirdPartyPath = Path.Combine(juniperPath, "ThirdParty");
            var juniperThirdPartyPackagePath = Path.Combine(juniperThirdPartyPath, "Optional");

            var rootDir = new DirectoryInfo(juniperThirdPartyPackagePath);
            foreach (var file in rootDir.GetFiles("*.zip"))
            {
                var packageName = Path.GetFileNameWithoutExtension(file.Name);
                var packagePath = file.FullName;
                packages.Add(new JuniperZipPackage(packageName, null, packagePath));
            }
        }

        public JuniperZipPackage(string name, string version, string path)
            : base(name, version, path)
        { }

        public override PackageSource Source => PackageSource.JuniperZip;

        protected override string InstallDirectory => Path.Combine(UnityProjectRoot, "Assets");

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
