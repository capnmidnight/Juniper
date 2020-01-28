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
        public static UnityPackageManifest Load()
        {
            return Load(out var _, out var __);
        }

        internal static UnityPackageManifest Load(out JsonFactory<UnityPackageManifest> factory, out string unityPackageManifestPath)
        {
            unityPackageManifestPath = Path.Combine(AbstractPackage.UnityProjectRoot, "Packages", "manifest.json");
            factory = new JsonFactory<UnityPackageManifest>();
            return factory.Deserialize(unityPackageManifestPath);
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
    }
}
