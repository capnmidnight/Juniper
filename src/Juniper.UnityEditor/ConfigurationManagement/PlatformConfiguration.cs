using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.XR;

using UnityEditor;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class PlatformConfiguration : ISerializable
    {
        public static PlatformConfiguration Current
        {
            get
            {
                var platforms = Platforms.Load();
                return platforms.Configurations[ProjectConfiguration.Platform];
            }
        }

        private static readonly string[] NO_VR_SYSTEMS = new string[] { "None" };

        public PlatformType Name { get; }

        public string CompilerDefine { get; }

        public string BuildTargetName { get; }

        public string Spatializer { get; }

        public string AndroidSdkVersion { get; }

        public string IOSVersion { get; }

        public string WsaSubtarget { get; }

        public IReadOnlyList<PackageReference> Packages { get; }

        public IReadOnlyList<string> VrSystems { get; }

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
            if(Enum.TryParse<PlatformType>(info.GetString(nameof(Name)), out var pType))
            {
                Name = pType;
            }

            BuildTargetName = info.GetString(nameof(BuildTargetName));
            VrSystems = NO_VR_SYSTEMS;

            foreach (var field in info)
            {
                switch (field.Name)
                {
                    case nameof(CompilerDefine): CompilerDefine = info.GetString(nameof(CompilerDefine)); break;
                    case nameof(VrSystems): VrSystems = info.GetValue<string[]>(nameof(VrSystems)); break;
                    case nameof(Packages): Packages = info.GetValue<string[]>(nameof(Packages)).Select(str => new PackageReference(str)).ToArray(); break;
                    case nameof(Spatializer): Spatializer = info.GetString(nameof(Spatializer)); break;
                    case nameof(AndroidSdkVersion): AndroidSdkVersion = info.GetString(nameof(AndroidSdkVersion)); break;
                    case nameof(IOSVersion): IOSVersion = info.GetString(nameof(IOSVersion)); break;
                    case nameof(WsaSubtarget): WsaSubtarget = info.GetString(nameof(WsaSubtarget)); break;
                }
            }

            if (Packages is null)
            {
                Packages = Array.Empty<PackageReference>();
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

            info.AddValue(nameof(Name), Name.ToString());
            info.AddValue(nameof(BuildTargetName), BuildTargetName);
            info.MaybeAddValue(nameof(CompilerDefine), CompilerDefine);
            info.MaybeAddValue(nameof(Spatializer), Spatializer);
            info.MaybeAddValue(nameof(AndroidSdkVersion), AndroidSdkVersion);
            info.MaybeAddValue(nameof(IOSVersion), IOSVersion);
            info.MaybeAddValue(nameof(WsaSubtarget), WsaSubtarget);
            info.MaybeAddValue(nameof(VrSystems), VrSystems.ToArray());
            info.MaybeAddValue(nameof(Packages), Packages.Select(p => p.PackageSpec).ToArray());
        }


        public BuildTarget GetBuildTarget()
        {
            var localBuildTarget = BuildTargetName;
            if (localBuildTarget == "Standalone")
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    return BuildTarget.StandaloneLinux64;
                }
                else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    return BuildTarget.StandaloneOSX;
                }
                else if (Environment.Is64BitOperatingSystem)
                {
                    return BuildTarget.StandaloneWindows64;
                }
                else
                {
                    return BuildTarget.StandaloneWindows;
                }
            }

            try
            {
                return (BuildTarget)Enum.Parse(typeof(BuildTarget), localBuildTarget);
            }
            catch
            {
                return BuildTarget.NoTarget;
            }
        }

        public BuildTargetGroup GetTargetGroup()
        {
            return BuildPipeline.GetBuildTargetGroup(GetBuildTarget());
        }
        
        public bool IsSupported()
        {
            return BuildPipeline.IsBuildTargetSupported(GetTargetGroup(), GetBuildTarget());
        }

        public bool IsActivated()
        {
            var curDefines = Project.GetDefines();
            var reqDefines = GetCompilerDefines();
            return reqDefines.All(curDefines.Contains);
        }

        public void SwitchTarget()
        {
            UnityEngine.Debug.Log($"Switching build target from {EditorUserBuildSettings.activeBuildTarget} to {BuildTargetName}.");
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(GetTargetGroup(), GetBuildTarget());
        }

        public bool IsTargetSwitchNeeded()
        {
            return GetBuildTarget() != EditorUserBuildSettings.activeBuildTarget;
        }

        public List<string> GetCompilerDefines()
        {
            var defines = new List<string>();

            if (!string.IsNullOrEmpty(CompilerDefine))
            {
                defines.MaybeAdd(CompilerDefine);
            }

            if (GetTargetGroup() == BuildTargetGroup.Android)
            {
                var target = PlayerSettings.Android.targetSdkVersion;
                if (target == AndroidSdkVersions.AndroidApiLevelAuto)
                {
                    target = Enum.GetValues(typeof(AndroidSdkVersions))
                        .Cast<AndroidSdkVersions>()
                        .Max();
                }
                for (var i = (int)target; i > 0; --i)
                {
                    if (Enum.IsDefined(typeof(AndroidSdkVersions), i))
                    {
                        defines.MaybeAdd("ANDROID_API_" + i);
                        defines.MaybeAdd("ANDROID_API_" + i + "_OR_GREATER");
                    }
                }
            }
            else if (GetTargetGroup() == BuildTargetGroup.iOS)
            {
                for (var i = ApplleiOS.TargetOSVersion.Major; i > 0; --i)
                {
                    defines.MaybeAdd("IOS_VERSION_" + i);
                    defines.MaybeAdd("IOS_VERSION_" + i + "_OR_GREATER");
                }
            }

            return defines
                .Distinct()
                .ToList();
        }

        public void Activate()
        {
            if (!string.IsNullOrEmpty(Spatializer))
            {
                var hasSpatializer = AudioSettings
                    .GetSpatializerPluginNames()
                    .Contains(Spatializer);

                if (hasSpatializer)
                {
                    AudioSettings.SetSpatializerPluginName(Spatializer);
                    AudioSettingsExt.SetAmbisonicDecoderPluginName(Spatializer);
                }
            }

            var buildTargetGroup = GetTargetGroup();
            var supportedVRSDKs = PlayerSettings.GetAvailableVirtualRealitySDKs(buildTargetGroup);
            var vrSDKs = XRPlatforms
                .Distinct()
                .Select(x => x.ToString())
                .Where(supportedVRSDKs.Contains)
                .ToArray();

            var enableVR = vrSDKs.Any(sdk => sdk != "None");
            if (enableVR != PlayerSettings.GetVirtualRealitySupported(buildTargetGroup))
            {
                PlayerSettings.SetVirtualRealitySupported(buildTargetGroup, enableVR);
                if (enableVR && !vrSDKs.Matches(PlayerSettings.GetVirtualRealitySDKs(buildTargetGroup)))
                {
                    PlayerSettings.SetVirtualRealitySDKs(buildTargetGroup, vrSDKs);
                }
            }

            if (buildTargetGroup == BuildTargetGroup.WSA)
            {
                EditorUserBuildSettings.wsaBuildAndRunDeployTarget = WSABuildAndRunDeployTarget.LocalMachine;
                EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
                PlayerSettings.WSA.inputSource = PlayerSettings.WSAInputSource.IndependentInputSource;
                if (Enum.TryParse(WsaSubtarget, out WSASubtarget sub))
                {
                    EditorUserBuildSettings.wsaSubtarget = sub;
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, sub == WSASubtarget.HoloLens);
                }
            }
            else if (buildTargetGroup == BuildTargetGroup.Android
                && Enum.TryParse(AndroidSdkVersion, out AndroidSdkVersions sdkVersion))
            {
                PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)Math.Max(
                    (int)PlayerSettings.Android.minSdkVersion,
                    (int)sdkVersion);
            }
            else if (buildTargetGroup == BuildTargetGroup.iOS
                && Version.TryParse(IOSVersion, out var v)
                && ApplleiOS.TargetOSVersion < v)
            {
                ApplleiOS.TargetOSVersion = v;
            }

            var curDefines = Project.GetDefines();
            curDefines.AddRange(GetCompilerDefines());
            Project.SetDefines(curDefines);
        }

        public static void Deactivate()
        {
            AudioSettings.SetSpatializerPluginName(null);
            AudioSettingsExt.SetAmbisonicDecoderPluginName(null);
            PlayerSettings.virtualRealitySupported = false;
            PlayerSettings.Android.androidTVCompatibility = false;
            PlayerSettings.Android.ARCoreEnabled = false;
            ApplleiOS.RequiresARKitSupport = false;
            PlayerSettings.vuforiaEnabled = false;
        }
    }
}
