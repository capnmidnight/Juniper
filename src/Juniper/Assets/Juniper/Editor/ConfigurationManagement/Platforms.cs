using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.Progress;
using Juniper.XR;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    internal sealed class Platforms : ISerializable
    {
        private const int MAX_CONCURRENT_PACKAGE_SCAN = 100;
        private static readonly string PACKAGE_DEFINES_FILE = PathExt.FixPath("Assets/Juniper/ThirdParty/defines.json");
        private static readonly string PLATFORMS_FILE = PathExt.FixPath("Assets/Juniper/platforms.json");
        private static readonly AbstractPackage[] EMPTY_PACKAGES = Array.Empty<AbstractPackage>();

        public static void ForEachPackage<T>(T[] packages, IProgress prog, Action<T, IProgress> act)
            where T : AbstractPackage
        {
            if (packages.Length > 0)
            {
                var progs = prog.Split(packages.Length);
                for (int i = 0; i < packages.Length; ++i)
                {
                    var pkg = packages[i];
                    var p = progs[i];

                    act?.Invoke(pkg, p);
                }
            }
        }

        private static readonly JsonFactory<PackageDefineSymbol[]> packageFactory = new JsonFactory<PackageDefineSymbol[]>();
        private static readonly JsonFactory<Platforms> platformFactory = new JsonFactory<Platforms>();
        private static readonly Dictionary<string, string> packageDefines = new Dictionary<string, string>();

        public static readonly Dictionary<string, AbstractFilePackage> packageDB;
        public static event Action<AbstractFilePackage[]> PackagesUpdated;
        public static event Action ScanningProgressUpdated;

        public static readonly PlatformConfiguration[] AllPlatforms;
        public static readonly Dictionary<PlatformTypes, PlatformConfiguration> PlatformDB;

        public static string[] AllCompilerDefines
        {
            get
            {
                return AllPlatforms
                    .SelectMany(p => p.CompilerDefines)
                    .Distinct()
                    .ToArray();
            }
        }

        public static void Save()
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
            foreach (var key in packageDefines.Keys)
            {
                if (!packageDB.ContainsKey(key))
                {
                    toRemove.Add(key);
                }
            }

            foreach (var key in toRemove)
            {
                packageDefines.Remove(key);
            }

            if (anyChanged)
            {
                var defines = (from kv in packageDefines
                               select new PackageDefineSymbol(kv.Key, kv.Value))
                            .ToArray();
                packageFactory.Serialize(PACKAGE_DEFINES_FILE, defines);

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

        static Platforms()
        {
            if (File.Exists(PACKAGE_DEFINES_FILE))
            {
                var defines = packageFactory.Deserialize<PackageDefineSymbol[]>(PACKAGE_DEFINES_FILE);
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

            var config = platformFactory.Deserialize<Platforms>(PLATFORMS_FILE);

            var commonPackages = ParsePackages(config.packages);

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

            AllPlatforms = config.platforms;
            PlatformDB = AllPlatforms.ToDictionary(pform => (PlatformTypes)Enum.Parse(typeof(PlatformTypes), pform.Name));

            foreach (var platform in AllPlatforms)
            {
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

        private static void FileWatcher()
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
                    if (scanningCount < MAX_CONCURRENT_PACKAGE_SCAN
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

        private static void Package_ScanningProgressUpdated()
        {
            ScanningProgressUpdated?.Invoke();
        }

        private static Task fileWatcherTask;

        public static void StartFileWatcher()
        {
            fileWatcherTask = Task.Run(FileWatcher);
        }

        public static bool IsRunning
        {
            get
            {
                return fileWatcherTask.IsRunning();
            }
        }

        private static AbstractPackage[] ParsePackages(string[] packages)
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

        private readonly string[] packages;
        private readonly PlatformConfiguration[] platforms;

        private Platforms(SerializationInfo info, StreamingContext context)
        {
            packages = info.GetValue<string[]>(nameof(packages));
            platforms = info.GetValue<PlatformConfiguration[]>(nameof(platforms));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(packages), packages);
            info.AddValue(nameof(platforms), platforms);
        }
    }
}
