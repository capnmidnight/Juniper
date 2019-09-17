using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Compression.Tar.GZip;
using Juniper.Progress;
using Juniper.XR;
using Juniper.Serialization;

using Json.Lite;
using Json.Lite.Linq;

using UnityEngine;
using Juniper.Json;

namespace Juniper.ConfigurationManagement
{
    internal class Platforms
    {
        private static readonly string PACKAGE_DEFINES_FILE = PathExt.FixPath("Assets/Juniper/ThirdParty/defines.json");
        private static readonly string[] NO_VR_SYSTEMS = new string[] { "None" };
        private static readonly AbstractPackage[] EMPTY_PACKAGES = new AbstractPackage[0];

        public static void ForEachPackage<T>(IEnumerable<T> packages, IProgress prog, Action<T, IProgress> act)
            where T : AbstractPackage
        {
            prog.ForEach(packages, (pkg, p) =>
                act?.Invoke(pkg, p), Debug.LogException);
        }

        private readonly JsonFactory json = new JsonFactory();
        private readonly Dictionary<string, string> packageDefines = new Dictionary<string, string>();

        public readonly Dictionary<string, AbstractFilePackage> packageDB;
        public event Action<AbstractFilePackage[]> PackagesUpdated;
        public event Action ScanningProgressUpdated;

        public readonly PlatformConfiguration[] AllPlatforms;
        public readonly Dictionary<PlatformTypes, PlatformConfiguration> PlatformDB;

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

        public void Save()
        {
            bool anyChanged = false;
            foreach (var package in packageDB.Values)
            {
                if (package.Changed)
                {
                    anyChanged = true;
                    packageDefines[package.Name] = package.CompilerDefine;
                }
            }

            var toRemove = new List<string>();
            foreach(var key in packageDefines.Keys)
            {
                if (!packageDB.ContainsKey(key))
                {
                    toRemove.Add(key);
                }
            }

            foreach(var key in toRemove)
            {
                packageDefines.Remove(key);
            }

            if (anyChanged)
            {
                var defines = (from kv in packageDefines
                               select new PackageDefineSymbol(kv.Key, kv.Value))
                            .ToArray();
                json.Save(PACKAGE_DEFINES_FILE, defines);

                foreach (var package in packageDB.Values)
                {
                    package.Save();
                }
            }
        }

        private static IEnumerable<AbstractFilePackage> GetPackages(
            Dictionary<string, AbstractFilePackage> curPackages,
            Dictionary<string, string> defines)
        {
            return AssetStorePackage.GetPackages(curPackages, defines)
                .Cast<AbstractFilePackage>()
                .Union(ZipPackage.GetPackages(curPackages, defines));
        }

        public Platforms()
        {
            if (File.Exists(PACKAGE_DEFINES_FILE))
            {
                var defines = json.Load<PackageDefineSymbol[]>(PACKAGE_DEFINES_FILE);
                foreach (var symbol in defines)
                {
                    packageDefines[symbol.Name] = symbol.CompilerDefine;
                }
            }

            packageDB = GetPackages(null, packageDefines).ToDictionary(pkg => pkg.Name);
            foreach (var package in packageDB.Values)
            {
                package.ScanningProgressUpdated += Package_ScanningProgressUpdated;
            }

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

            var commonZipPackages = commonPackages
                .OfType<ZipPackage>()
                .ToArray();

            var commonAssetStorePackages = commonPackages
                .OfType<AssetStorePackage>()
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

                platform.UninstallableZipPackages = packages
                    .OfType<ZipPackage>()
                    .ToArray();

                platform.ZipPackages = platform
                    .UninstallableZipPackages
                    .Union(commonZipPackages)
                    .ToArray();

                platform.UninstallableAssetStorePackages = packages
                    .OfType<AssetStorePackage>()
                    .ToArray();

                platform.AssetStorePackages = platform
                    .UninstallableAssetStorePackages
                    .Union(commonAssetStorePackages)
                    .ToArray();
            }
        }

        private void FileWatcher()
        {
            while (true)
            {
                var packages = (from pkg in GetPackages(packageDB, packageDefines)
                                where !packageDB.ContainsKey(pkg.Name)
                                select pkg);

                foreach (var package in packages)
                {
                    packageDB[package.Name] = package;
                    package.ScanningProgressUpdated += Package_ScanningProgressUpdated;
                }

                int scanningCount = 0;
                foreach (var p in packageDB.Values)
                {
                    if (p.ScanningProgress == PackageScanStatus.Scanning
                        || p.ScanningProgress == PackageScanStatus.Listing)
                    {
                        ++scanningCount;
                    }
                }

                foreach (var p in packageDB.Values)
                {
                    if (scanningCount < 4
                        || (p.ScanningProgress != PackageScanStatus.List
                            && p.ScanningProgress != PackageScanStatus.Scan))
                    {
                        if (p.ScanningProgress == PackageScanStatus.List
                            || p.ScanningProgress == PackageScanStatus.Scan)
                        {
                            ++scanningCount;
                        }
                        p.Update();
                    }
                }

                PackagesUpdated?.Invoke(packageDB.Values.ToArray());
            }
        }

        private void Package_ScanningProgressUpdated()
        {
            ScanningProgressUpdated?.Invoke();
        }

        private Task fileWatcherTask;

        public void StartFileWatcher()
        {
            fileWatcherTask = Task.Run(FileWatcher);
            fileWatcherTask.ConfigureAwait(false);
        }

        public bool IsRunning { get { return fileWatcherTask.IsRunning(); } }

        private AbstractPackage[] ParsePackages(string[] packages)
        {
            if (packages == null)
            {
                return EMPTY_PACKAGES;
            }
            else
            {
                return (from pkgDef in packages
                        let isAssetStore = pkgDef.EndsWith(".unitypackage")
                        let isUnity = pkgDef.Contains('@')
                        let isZip = !isAssetStore && !isUnity
                        let pkgName = Path.GetFileNameWithoutExtension(pkgDef)
                        select isAssetStore || isZip
                            ? (AbstractPackage)packageDB.Get(pkgName)
                            : new UnityPackage(pkgDef))
                    .ToArray();
            }
        }
    }
}
