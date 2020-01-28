using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;
using Juniper.XR;

using UnityEditor;

using UnityEngine;

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
        public string[] Packages { get; }

        public string[] VrSystems { get; }

        public IEnumerable<UnityXRPlatform> XRPlatforms
        {
            get
            {
                return from sys in VrSystems
                       select (UnityXRPlatform)Enum.Parse(typeof(UnityXRPlatform), sys);
            }
        }

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
                    case nameof(Packages): Packages = info.GetValue<string[]>(nameof(Packages)); break;
                    case nameof(Spatializer): Spatializer = info.GetString(nameof(Spatializer)); break;
                    case nameof(AndroidSdkVersion): AndroidSdkVersion = info.GetString(nameof(AndroidSdkVersion)); break;
                    case nameof(IOSVersion): IOSVersion = info.GetString(nameof(IOSVersion)); break;
                    case nameof(WsaSubtarget): WsaSubtarget = info.GetString(nameof(WsaSubtarget)); break;
                }
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
            info.MaybeAddValue(nameof(Packages), Packages);
        }
    }
}
