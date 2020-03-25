using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper.ConfigurationManagement
{
    /// <summary>
    /// </summary>
    /// <remarks>Don't give this class the <see cref="IDictionary{string, string}"/> interface,
    /// as it breaks the deserialization process.</remarks>
    [Serializable]
    public sealed class UnityPackageManifest :
        ISerializable
    {
        private static UnityPackageManifest manifest;

        public static UnityPackageManifest Load()
        {
            if (manifest is null)
            {
                var unityPackageManifestPath = Path.Combine(Project.UnityProjectRoot, "Packages", "manifest.json");
                using var stream = FileDataSource.Instance.GetStream(unityPackageManifestPath);
                var factory = new JsonFactory<UnityPackageManifest>();
                manifest = factory.Deserialize(stream);
            }

            return manifest;
        }

        public void Save()
        {
            var unityPackageManifestPath = Path.Combine(Project.UnityProjectRoot, "Packages", "manifest.json");
            var factory = new JsonFactory<UnityPackageManifest>();
            factory.Serialize(unityPackageManifestPath, this);
        }


        private readonly Dictionary<string, PackageReference> dependencies;

        public ICollection<string> Keys => dependencies.Keys;

        public ICollection<PackageReference> Values => dependencies.Values;

        public PackageReference this[string key]
        {
            get
            {
                return dependencies[key];
            }

            set
            {
                dependencies[key] = value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private UnityPackageManifest(SerializationInfo info, StreamingContext context)
        {
            var dict = info.GetValue<Dictionary<string, string>>(nameof(dependencies));
            dependencies = dict.ToDictionary(kv => kv.Key, kv => new PackageReference(PackageReference.FormatUnityPackageManagerSpec(kv.Key, kv.Value)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var dict = dependencies.ToDictionary(kv => kv.Key, kv => kv.Value.VersionSpec);
            info.AddValue(nameof(dependencies), dict);
        }

        public bool ContainsKey(string key)
        {
            return dependencies.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return dependencies.Remove(key);
        }

        public bool TryGetValue(string key, out PackageReference value)
        {
            return dependencies.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dependencies.Clear();
        }

        public void Add(PackageReference pkg)
        {
            if (pkg is null)
            {
                throw new ArgumentNullException(nameof(pkg));
            }

            if (pkg.ForRemoval)
            {
                Remove(pkg);
            }
            else
            {
                dependencies[pkg.PackageID] = pkg;
                Save();
            }
        }

        public void Remove(PackageReference pkg)
        {
            if (pkg is null)
            {
                throw new ArgumentNullException(nameof(pkg));
            }

            if (dependencies.ContainsKey(pkg.PackageID))
            {
                dependencies.Remove(pkg.PackageID);
            }

            Save();
        }
    }
}
