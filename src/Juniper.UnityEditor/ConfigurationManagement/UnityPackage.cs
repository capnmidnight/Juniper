using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.IO;

namespace Juniper.ConfigurationManagement
{
    public sealed class UnityPackage : AbstractPackage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="listingRoot">version in subdir name, contents in subdir/package.tgz, version and displayName also in subdir/package/package.json</param>
        /// <param name="contentCacheRoot">version number = "dir name@version", contents in dir, version and displayName in package.json</param>
        public static void Load(List<AbstractPackage> packages)
        {
            if (packages is null)
            {
                throw new ArgumentNullException(nameof(packages));
            }

            var packageFactory = new JsonFactory<NpmPackage>();

            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var unityCachePath = Path.Combine(localAppDataPath, "Unity", "cache");
            var unityPackageListingRoot = Path.Combine(unityCachePath, "npm", "packages.unity.com"); // version in subdir name, contents in subdir/package.tgz, version and displayName also in subdir/package/package.json
            var unityPackageContentCacheRoot = Path.Combine(unityCachePath, "packages", "packages.unity.com"); // version number = "dir name@version", contents in dir, version and displayName in package.json

            var listingRootDir = new DirectoryInfo(unityPackageListingRoot);

            foreach (var packageDir in listingRootDir.GetDirectories())
            {
                if (packageDir.Name != "packages.unity.com")
                {
                    foreach (var versionDir in packageDir.GetDirectories())
                    {
                        var packageFileName = Path.Combine(versionDir.FullName, "package", "package.json");
                        using var stream = FileDataSource.Instance.GetStream(packageFileName);
                        var package = packageFactory.Deserialize(stream);
                        var packageName = package.DisplayName;
                        if (packageName.StartsWith("Oculus", StringComparison.OrdinalIgnoreCase))
                        {
                            packageName = packageName.Replace("(", "")
                                .Replace(")", "");
                        }
                        var version = package.Version;
                        var packagePath = Path.Combine(unityPackageContentCacheRoot, $"{packageDir.Name}@{version}");
                        packages.Add(new UnityPackage(package.Name, packageName, version, versionDir.FullName, packagePath));
                    }
                }
            }

            if (Project.UnityEditorRoot != null)
            {
                var builtInRoot = Path.Combine(Project.UnityEditorRoot, "Editor", "Data", "Resources", "PackageManager", "BuiltInPackages");
                var builtInRootDir = new DirectoryInfo(builtInRoot);
                foreach (var packageDir in builtInRootDir.GetDirectories())
                {
                    var packageFileName = Path.Combine(packageDir.FullName, "package.json");
                    using var stream = FileDataSource.Instance.GetStream(packageFileName);
                    var package = packageFactory.Deserialize(stream);
                    var packageName = package.DisplayName;
                    var version = package.Version;
                    packages.Add(new UnityPackage(package.Name, packageName, version, packageDir.FullName));
                }
            }
        }

        public static string MakeCompilerDefine(string packageID)
        {
            if (packageID is null)
            {
                throw new ArgumentNullException(nameof(packageID));
            }

            var parts = packageID.Split('.');
            return string.Join("_", parts.Skip(1)).ToUpperInvariant();
        }

        public string ListingPath { get; }

        public UnityPackage(string packageID, string name, string version, string listingPath, string contentPath = null)
            : base(PackageSources.UnityPackageManager, packageID, name, version, contentPath, MakeCompilerDefine(packageID))
        {
            ListingPath = listingPath ?? throw new ArgumentNullException(nameof(listingPath));
        }

        public override bool Available => true;

        public override bool Cached => Directory.Exists(ContentPath);

        public override float InstallPercentage =>
            IsInstalled ? 100 : 0;

        public override bool IsInstalled
        {
            get
            {
                var manifest = UnityPackageManifest.Load();
                return manifest.ContainsKey(PackageID)
                    && manifest[PackageID].VersionSpec == VersionSpec;
            }
        }

        public override bool CanUpdate
        {
            get
            {
                var manifest = UnityPackageManifest.Load();
                if (manifest.ContainsKey(PackageID))
                {
                    var installed = manifest[PackageID];
                    return Version is object
                        && (installed.Version is null
                            || Version > installed.Version);
                }

                return true;
            }
        }

        public override void Install()
        {
            var manifest = UnityPackageManifest.Load();

            if (!ForRemoval)
            {
                manifest.Add(this);
            }
            else if (manifest.ContainsKey(PackageID))
            {
                manifest.Remove(this);
            }
        }
    }
}
