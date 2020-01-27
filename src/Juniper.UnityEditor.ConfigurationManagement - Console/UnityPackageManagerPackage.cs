using System;
using System.Collections.Generic;
using System.IO;

using Juniper.IO;

namespace Juniper.ConfigurationManagement
{
    public class UnityPackageManagerPackage : AbstractPackage2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="listingRoot">version in subdir name, contents in subdir/package.tgz, version and displayName also in subdir/package/package.json</param>
        /// <param name="contentCacheRoot">version number = "dir name@version", contents in dir, version and displayName in package.json</param>
        public static void GetPackages(List<AbstractPackage2> packages)
        {
            if (packages is null)
            {
                throw new ArgumentNullException(nameof(packages));
            }

            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var unityCachePath = Path.Combine(localAppDataPath, "Unity", "cache");
            var unityPackageListingRoot = Path.Combine(unityCachePath, "npm", "packages.unity.com"); // version in subdir name, contents in subdir/package.tgz, version and displayName also in subdir/package/package.json
            var unityPackageContentCacheRoot = Path.Combine(unityCachePath, "packages", "packages.unity.com"); // version number = "dir name@version", contents in dir, version and displayName in package.json

            var listingRootDir = new DirectoryInfo(unityPackageListingRoot);
            var packageFactory = new JsonFactory<NpmPackage>();

            foreach (var packageDir in listingRootDir.GetDirectories())
            {
                if (packageDir.Name != "packages.unity.com")
                {
                    foreach (var versionDir in packageDir.GetDirectories())
                    {
                        var packageFileName = Path.Combine(versionDir.FullName, "package", "package.json");
                        var package = packageFactory.Deserialize(packageFileName);
                        var packageName = package.DisplayName;
                        if (packageName.StartsWith("Oculus", StringComparison.OrdinalIgnoreCase))
                        {
                            packageName = packageName.Replace("(", "")
                                .Replace(")", "");
                        }
                        var version = package.Version;
                        var packagePath = Path.Combine(unityPackageContentCacheRoot, $"{packageDir.Name}@{version}");
                        packages.Add(new UnityPackageManagerPackage(packageName, package.Name, version, packagePath, versionDir.FullName));
                    }
                }
            }
        }

        public string PackageID { get; }
        public string ListingPath { get; }
        public UnityPackageManagerPackage(string name, string packageID, string version, string contentPath, string listingPath)
            : base(name, version, contentPath)
        {
            PackageID = packageID;
            ListingPath = listingPath;
        }

        public override PackageSource Source => PackageSource.UnityPackageManager;

        public override bool Available => Directory.Exists(ListingPath);

        public override bool Cached => Directory.Exists(ContentPath);

        private static UnityPackageManifest LoadManifest()
        {
            var unityPackageManifestPath = Path.Combine(UnityProjectRoot, "Packages", "manifest.json");
            var factory = new JsonFactory<UnityPackageManifest>();
            return factory.Deserialize(unityPackageManifestPath);
        }

        public override bool IsInstalled
        {
            get
            {
                var manifest = LoadManifest();
                return manifest.ContainsKey(PackageID)
                    && manifest[PackageID] == Version;
            }
        }

        public override bool CanUpdate
        {
            get
            {
                var manifest = LoadManifest();
                if (manifest.ContainsKey(PackageID))
                {
                    var installedVersionStr = manifest[PackageID];
                    var isInstalledValidVersion = System.Version.TryParse(installedVersionStr, out var iv);
                    var isThisValidVersion = System.Version.TryParse(Version, out var v);
                    return isThisValidVersion && !isInstalledValidVersion
                        || isThisValidVersion && isInstalledValidVersion && v > iv;
                }

                return true;
            }
        }

        private static void SetManifestField(string package, string version)
        {
            if (package is null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var unityPackageManifestPath = Path.Combine(UnityProjectRoot, "Packages", "manifest.json");
            var factory = new JsonFactory<UnityPackageManifest>();
            var manifest = factory.Deserialize(unityPackageManifestPath);

            if (version is object)
            {
                manifest[package] = version;
            }
            else if (manifest.ContainsKey(package))
            {
                manifest.Remove(package);
            }

            factory.Serialize(unityPackageManifestPath, manifest);
        }

        public override void Install()
        {
            SetManifestField(PackageID, Version);
        }

        public void Uninstall()
        {
            SetManifestField(PackageID, null);
        }
    }
}
