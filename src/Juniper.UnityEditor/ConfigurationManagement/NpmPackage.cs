using System;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class NpmPackage : ISerializable
    {
        public string Name { get; }

        public string DisplayName { get; }

        public string Version { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private NpmPackage(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("name");
            foreach (var field in info)
            {
                if (field.Name.Equals(nameof(DisplayName), StringComparison.OrdinalIgnoreCase))
                {
                    DisplayName = info.GetString(field.Name);
                }
                else if (field.Name.Equals(nameof(Version), StringComparison.OrdinalIgnoreCase))
                {
                    Version = info.GetString(field.Name);
                }
            }

            DisplayName ??= Name;
            Version ??= "unityPackage";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
