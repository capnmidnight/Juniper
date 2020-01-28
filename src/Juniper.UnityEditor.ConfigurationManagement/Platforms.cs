using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Juniper.IO;
using Juniper.XR;

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
        public static readonly Dictionary<PlatformType, PlatformConfiguration> PlatformDB;

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

        static Platforms()
        {
            if (File.Exists(PACKAGE_DEFINES_FILE))
            {
                var defines = packageFactory.Deserialize(PACKAGE_DEFINES_FILE);
                foreach (var symbol in defines)
                {
                    packageDefines[symbol.Name] = symbol.CompilerDefine;
                }
            }

          

            var config = platformFactory.Deserialize(PLATFORMS_FILE);

           
            AllPlatforms = config.platforms;
            PlatformDB = AllPlatforms.ToDictionary(pform => (PlatformType)Enum.Parse(typeof(PlatformType), pform.Name));
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
