using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class UnityPackageManifest :
        ISerializable
    {
        public Dictionary<string, string> dependencies { get; }

        public ICollection<string> Keys => ((IDictionary<string, string>)dependencies).Keys;

        public string this[string key]
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
            dependencies = info.GetValue<Dictionary<string, string>>(nameof(dependencies));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(dependencies), dependencies);
        }

        public bool ContainsKey(string key)
        {
            return dependencies.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return dependencies.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return dependencies.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dependencies.Clear();
        }
    }
}
