using Juniper.Progress;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

namespace Juniper.Unity.ConfigurationManagement
{
    internal class Platforms
    {
        public static Platforms Instance;

        static Platforms()
        {
            Instance = new Platforms();
        }

        public static void ForEachPackage<T>(IEnumerable<T> packages, IProgressReceiver prog, Action<T, IProgressReceiver> act)
            where T : AbstractPackage
        {
            prog.ForEach(packages, (pkg, p) =>
                act?.Invoke(pkg, p), Debug.LogException);
        }

        public static string[] GetCompilerDefines(UnityPackage[] unityPackages, RawPackage[] rawPackages)
        {
            return (from pkg in unityPackages
                    where pkg.version != "exclude"
                    select pkg.CompilerDefine)
                .Union(from pkg in rawPackages
                       select pkg.CompilerDefine)
                .Where(def => !string.IsNullOrEmpty(def))
                .ToArray();
        }

        public readonly RawPackage[] allRawPackages;
        public readonly Dictionary<string, AbstractPackage> rawPackageDB;

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

        public Platforms()
        {
            var rawPackageJson = File.ReadAllText(Path.Combine(RawPackage.ROOT_DIRECTORY, "packages.json"));
            allRawPackages = JsonConvert.DeserializeObject<RawPackage[]>(rawPackageJson);
            rawPackageDB = allRawPackages.Cast<AbstractPackage>().ToDictionary(pkg => pkg.Name);

            var platformsJson = File.ReadAllText(PathExt.FixPath("Assets/Juniper/platforms.json"));
            var config = JObject.Parse(platformsJson);
            var common = config["packages"] as JArray;
            var commonPackageDefs = (from token in common
                                     select token.ToString()).ToArray();
            var commonPackages = ParsePackages(commonPackageDefs);
            var commonUnityPackages = commonPackages.OfType<UnityPackage>().ToArray();
            var includedUnityPackages = commonUnityPackages.Where(p => p.version != "exclude").ToArray();
            var excludedUnityPackages = commonUnityPackages.Where(p => p.version == "exclude").ToArray();
            var rawPackages = commonUnityPackages.OfType<RawPackage>().ToArray();

            var platforms = config["platforms"];
            AllPlatforms = JsonConvert.DeserializeObject<PlatformConfiguration[]>(platforms.ToString());
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

                platform.UninstallableRawPackages = packages
                    .OfType<RawPackage>()
                    .ToArray();

                platform.RawPackages = platform
                    .UninstallableRawPackages
                    .Union(rawPackages)
                    .ToArray();
            }
        }

        private AbstractPackage[] ParsePackages(string[] packages)
        {
            return (from pkgName in packages
                    select rawPackageDB.Get(pkgName)
                    ?? new UnityPackage(pkgName))
                .ToArray();
        }
    }
}
