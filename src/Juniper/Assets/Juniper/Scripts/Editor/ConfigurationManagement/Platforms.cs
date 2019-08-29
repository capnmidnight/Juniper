using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Compression.Tar.GZip;
using Juniper.Progress;
using Juniper.XR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    internal class Platforms
    {
        private static readonly string[] NO_VR_SYSTEMS = new string[] { "None" };

        private static readonly AbstractPackage[] EMPTY_PACKAGES = new AbstractPackage[0];

        public static void ForEachPackage<T>(IEnumerable<T> packages, IProgress prog, Action<T, IProgress> act)
            where T : AbstractPackage
        {
            prog.ForEach(packages, (pkg, p) =>
                act?.Invoke(pkg, p), Debug.LogException);
        }

        public static List<string> GetCompilerDefines(UnityPackage[] unityPackages, ZipPackage[] rawPackages)
        {
            return (from pkg in unityPackages
                    where pkg.version != "exclude"
                    select pkg.CompilerDefine)
                .Union(from pkg in rawPackages
                       select pkg.CompilerDefine)
                .Where(def => !string.IsNullOrEmpty(def))
                .Distinct()
                .ToList();
        }

        public readonly ZipPackage[] allRawPackages;
        public readonly Dictionary<string, AbstractPackage> rawPackageDB;

        public readonly List<AssetStorePackage> allAssetStorePackages;
        private readonly Dictionary<string, DateTime> writeTimes;

        public readonly PlatformConfiguration[] AllPlatforms;
        public readonly Dictionary<PlatformTypes, PlatformConfiguration> PlatformDB;

        public event Action<AssetStorePackage[]> AssetStorePackagesUpdated;
        public event Action ScanningProgressUpdated;

        public string[] AllCompilerDefines
        {
            get
            {
                return AllPlatforms
                    .SelectMany(p => p.CompilerDefines)
                    .Distinct()
                    .ToArray();
            }
        }

        public Platforms()
        {
            writeTimes = new Dictionary<string, DateTime>();
            allAssetStorePackages = new List<AssetStorePackage>();

            var rawPackageJson = File.ReadAllText(Path.Combine(ZipPackage.ROOT_DIRECTORY, "packages.json"));
            allRawPackages = JsonConvert.DeserializeObject<ZipPackage[]>(rawPackageJson);
            rawPackageDB = allRawPackages.Cast<AbstractPackage>().ToDictionary(pkg => pkg.Name);

            var platformsJson = File.ReadAllText(PathExt.FixPath("Assets/Juniper/platforms.json"));
            var config = JObject.Parse(platformsJson);

            var common = config["packages"] as JArray;
            var commonPackageDefs = (from token in common
                                     select token.ToString()).ToArray();

            var commonPackages = ParsePackages(commonPackageDefs);

            var commonUnityPackages = commonPackages
                .OfType<UnityPackage>()
                .ToArray();
            var includedUnityPackages = commonUnityPackages
                .Where(p => p.version != "exclude")
                .ToArray();
            var excludedUnityPackages = commonUnityPackages
                .Where(p => p.version == "exclude")
                .ToArray();

            var commonRawPackages = commonPackages
                .OfType<ZipPackage>()
                .ToArray();

            var platforms = config["platforms"];
            AllPlatforms = JsonConvert.DeserializeObject<PlatformConfiguration[]>(platforms.ToString());
            PlatformDB = AllPlatforms.ToDictionary(pform => (PlatformTypes)Enum.Parse(typeof(PlatformTypes), pform.Name));

            foreach (var platform in AllPlatforms)
            {
                if (platform.vrSystems == null)
                {
                    platform.vrSystems = NO_VR_SYSTEMS;
                }
                var packages = ParsePackages(platform.packages);
                var unityPackages = packages.OfType<UnityPackage>().ToArray();

                platform.UninstallableUnityPackages = unityPackages
                    .Where(p => p.version != "exclude")
                    .ToArray();

                platform.IncludedUnityPackages = platform
                    .UninstallableUnityPackages
                    .Union(includedUnityPackages)
                    .ToArray();

                platform.ExcludedUnityPackages = unityPackages
                    .Where(p => p.version == "exclude")
                    .Union(excludedUnityPackages)
                    .ToArray();

                platform.UninstallableRawPackages = packages
                    .OfType<ZipPackage>()
                    .ToArray();

                platform.RawPackages = platform
                    .UninstallableRawPackages
                    .Union(commonRawPackages)
                    .ToArray();
            }
        }

        private void FileWatcher()
        {
            while (true)
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var packages = Decompressor.FindUnityPackages(
                    Path.Combine(userProfile, "AppData", "Roaming", "Unity", "Asset Store-5.x"),
                    Path.Combine(userProfile, "Projects", "Packages"));
                var package = (from path in packages
                               let file = new FileInfo(path)
                               where file.LastWriteTime > writeTimes.Get(file.FullName, DateTime.MinValue)
                               select new AssetStorePackage(file))
                            .FirstOrDefault();
                if (package != null)
                {
                    writeTimes[package.InputUnityPackageFile] = package.LastWriteTime;
                    allAssetStorePackages.Add(package);
                    package.ScanningProgressUpdated += Package_ScanningProgressUpdated;
                }

                int scanningCount = 0;
                foreach (var p in allAssetStorePackages)
                {
                    if (p.ScanningProgress == AssetStorePackage.Status.Scanning
                        || p.ScanningProgress == AssetStorePackage.Status.Listing)
                    {
                        ++scanningCount;
                    }
                }

                foreach (var p in allAssetStorePackages)
                {
                    if(scanningCount < 4
                        || (p.ScanningProgress != AssetStorePackage.Status.List
                            && p.ScanningProgress != AssetStorePackage.Status.Scan))
                    {
                        if (p.ScanningProgress == AssetStorePackage.Status.List
                            || p.ScanningProgress == AssetStorePackage.Status.Scan)
                        {
                            ++scanningCount;
                        }
                        p.Update();
                    }
                }

                AssetStorePackagesUpdated?.Invoke(allAssetStorePackages.ToArray());
            }
        }

        private void Package_ScanningProgressUpdated()
        {
            ScanningProgressUpdated?.Invoke();
        }

        public void StartFileWatcher()
        {
            Task.Run(FileWatcher);
        }

        private AbstractPackage[] ParsePackages(string[] packages)
        {
            if (packages == null)
            {
                return EMPTY_PACKAGES;
            }
            else
            {
                return (from pkgName in packages
                        select rawPackageDB.Get(pkgName)
                        ?? new UnityPackage(pkgName))
                    .ToArray();
            }
        }
    }
}
