using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.XR;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class PlatformConfiguration : ISerializable
    {
        private static readonly string[] NO_VR_SYSTEMS = new string[] { "None" };

        public string Name { get; }

        public string CompilerDefine { get; }

        public string BuildTarget { get; }

        public string Spatializer { get; }

        public string AndroidSdkVersion { get; }

        public string IOSVersion { get; }

        public string WsaSubtarget { get; }

        public PackageRequirement[] Packages { get; }

        public string[] VrSystems { get; }

        public IEnumerable<UnityXRPlatform> XRPlatforms
        {
            get
            {
                return from sys in VrSystems
                       select (UnityXRPlatform)Enum.Parse(typeof(UnityXRPlatform), sys);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        private PlatformConfiguration(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString(nameof(Name));
            BuildTarget = info.GetString(nameof(BuildTarget));
            VrSystems = NO_VR_SYSTEMS;

            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(CompilerDefine): CompilerDefine = info.GetString(nameof(CompilerDefine)); break;
                    case nameof(VrSystems): VrSystems = info.GetValue<string[]>(nameof(VrSystems)); break;
                    case nameof(Packages): Packages = info.GetValue<string[]>(nameof(Packages)).Select(str => new PackageRequirement(str)).ToArray(); break;
                    case nameof(Spatializer): Spatializer = info.GetString(nameof(Spatializer)); break;
                    case nameof(AndroidSdkVersion): AndroidSdkVersion = info.GetString(nameof(AndroidSdkVersion)); break;
                    case nameof(IOSVersion): IOSVersion = info.GetString(nameof(IOSVersion)); break;
                    case nameof(WsaSubtarget): WsaSubtarget = info.GetString(nameof(WsaSubtarget)); break;
                }
            }

            if (Packages is null)
            {
                Packages = Array.Empty<PackageRequirement>();
            }

            if (VrSystems is null)
            {
                VrSystems = Array.Empty<string>();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(BuildTarget), BuildTarget);
            info.MaybeAddValue(nameof(CompilerDefine), CompilerDefine);
            info.MaybeAddValue(nameof(Spatializer), Spatializer);
            info.MaybeAddValue(nameof(AndroidSdkVersion), AndroidSdkVersion);
            info.MaybeAddValue(nameof(IOSVersion), IOSVersion);
            info.MaybeAddValue(nameof(WsaSubtarget), WsaSubtarget);
            info.MaybeAddValue(nameof(VrSystems), VrSystems);
            info.MaybeAddValue(nameof(Packages), Packages.Select(p => p.PackageSpec).ToArray());
        }
    }
}
