using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;
using Juniper.XR;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class Platforms : ISerializable
    {
        private static Platforms platforms;
        public static Platforms Load()
        {
            if (platforms is null)
            {
                var configFactory = new JsonFactory<Platforms>();
                platforms = configFactory.Deserialize(Project.JuniperPlatformsFileName);
            }

            return platforms;
        }

        public IReadOnlyList<PackageReference> Packages { get; }

        public IReadOnlyDictionary<PlatformType, PlatformConfiguration> Configurations { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private Platforms(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            Packages = info.GetValue<string[]>(nameof(Packages))
                .Select(str => new PackageReference(str))
                .ToArray();
            Configurations = info.GetValue<PlatformConfiguration[]>(nameof(Configurations))
                .ToDictionary(c => c.Name);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Packages), Packages.Select(p => p.PackageSpec).ToArray());
            info.AddValue(nameof(Configurations), Configurations.Values.ToArray());
        }
    }
}
