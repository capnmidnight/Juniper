using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class Platforms : ISerializable
    {
        public PackageRequirement[] Packages { get; }

        public PlatformConfiguration[] Configurations { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Platforms(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Packages = info.GetValue<string[]>(nameof(Packages))
                .Select(str => new PackageRequirement(str))
                .ToArray();
            Configurations = info.GetValue<PlatformConfiguration[]>(nameof(Configurations));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Packages), Packages.Select(p => p.PackageSpec).ToArray());
            info.AddValue(nameof(Configurations), Configurations);
        }
    }
}
