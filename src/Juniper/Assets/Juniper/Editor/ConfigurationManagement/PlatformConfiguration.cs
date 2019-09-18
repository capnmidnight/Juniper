using System;
using System.Collections.Generic;
using System.Linq;

using Juniper.Progress;
using Juniper.XR;

using Json.Lite;

using UnityEditor;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    internal sealed class PlatformConfiguration
    {
        public string Name;
        public string CompilerDefine;
        public string buildTarget;
        public string spatializer;
        public string androidSdkVersion;
        public string iOSVersion;
        public string wsaSubtarget;
        public string[] vrSystems;
        public string[] packages;

        public PlatformConfiguration()
        {
            Name = null;
            CompilerDefine = null;
            buildTarget = null;
            vrSystems = null;
            packages = null;
            spatializer = null;
            androidSdkVersion = null;
            iOSVersion = null;
            wsaSubtarget = null;
        }

        [JsonIgnore]
        public BuildTarget BuildTarget
        {
            get
            {
                var localBuildTarget = buildTarget;
                if (localBuildTarget == "Standalone")
                {
#if UNITY_EDITOR_WIN
                    if (Environment.Is64BitOperatingSystem)
                    {
                        localBuildTarget = nameof(BuildTarget.StandaloneWindows64);
                    }
                    else
                    {
                        localBuildTarget = nameof(BuildTarget.StandaloneWindows);
                    }
#elif UNITY_EDITOR_OSX
                    localBuildTarget = nameof(BuildTarget.StandaloneOSX);
#else
                    if (Environment.Is64BitOperatingSystem)
                    {
                        localBuildTarget = nameof(BuildTarget.StandaloneLinux64);
                    }
                    else
                    {
                        localBuildTarget = nameof(BuildTarget.StandaloneLinux);
                    }
#endif
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
        }

        [JsonIgnore]
        public BuildTargetGroup TargetGroup
        {
            get
            {
                return BuildPipeline.GetBuildTargetGroup(BuildTarget);
            }
        }

        [JsonIgnore]
        public bool IsSupported
        {
            get
            {
                return BuildPipeline.IsBuildTargetSupported(TargetGroup, BuildTarget);
            }
        }

        [JsonIgnore]
        public IEnumerable<UnityXRPlatforms> XRPlatforms
        {
            get
            {
                return from sys in vrSystems
                       select (UnityXRPlatforms)Enum.Parse(typeof(UnityXRPlatforms), sys);
            }
        }

        [JsonIgnore]
        public UnityPackage[] UninstallableUnityPackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public UnityPackage[] IncludedUnityPackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public UnityPackage[] ExcludedUnityPackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public ZipPackage[] UninstallableZipPackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public ZipPackage[] ZipPackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public AssetStorePackage[] UninstallableAssetStorePackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public AssetStorePackage[] AssetStorePackages
        {
            get; internal set;
        }

        public void SwitchTarget()
        {
            Debug.Log($"Switching build target from {EditorUserBuildSettings.activeBuildTarget} to {BuildTarget}.");
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(TargetGroup, BuildTarget);
        }

        public bool TargetSwitchNeeded
        {
            get { return BuildTarget != EditorUserBuildSettings.activeBuildTarget; }
        }

        public void InstallUnityPackages(IProgress prog)
        {
            var progs = prog.Split(2);

            Platforms.ForEachPackage(ExcludedUnityPackages, progs[0], (pkg, p) =>
            {
                pkg.Uninstall(p);
            });

            Platforms.ForEachPackage(IncludedUnityPackages, progs[1], (pkg, p) =>
            {
                pkg.Install(p);
            });
        }

        public void InstallZipPackages(IProgress prog)
        {
            Platforms.ForEachPackage(ZipPackages, prog, (pkg, p) => pkg.Install(p));
        }

        public void InstallAssetStorePackages(IProgress prog)
        {
            Platforms.ForEachPackage(AssetStorePackages, prog, (pkg, p) => pkg.Install(p));
        }

        public void Activate(IProgress prog)
        {
            var progs = prog.Split(2);

            Platforms.ForEachPackage(
                IncludedUnityPackages, progs[0],
                (pkg, p) => pkg.Activate(TargetGroup, p));

            Platforms.ForEachPackage(
                ZipPackages, progs[1],
                (pkg, p) => pkg.Activate(TargetGroup, p));

            Platforms.ForEachPackage(
                AssetStorePackages, progs[1],
                (pkg, p) => pkg.Activate(TargetGroup, p));

            if (!string.IsNullOrEmpty(spatializer))
            {
                var hasSpatializer = AudioSettings
                    .GetSpatializerPluginNames()
                    .Contains(spatializer);

                if (hasSpatializer)
                {
                    AudioSettings.SetSpatializerPluginName(spatializer);
                    AudioSettingsExt.SetAmbisonicDecoderPluginName(spatializer);
                }
            }

            var supportedVRSDKs = PlayerSettings.GetAvailableVirtualRealitySDKs(TargetGroup);
            var vrSDKs = XRPlatforms
                .Distinct()
                .Select(x => x.ToString())
                .Where(supportedVRSDKs.Contains)
                .ToArray();

            var enableVR = vrSDKs.Any(sdk => sdk != "None");
            if (enableVR != PlayerSettings.GetVirtualRealitySupported(TargetGroup))
            {
                PlayerSettings.SetVirtualRealitySupported(TargetGroup, enableVR);
                if (enableVR && !vrSDKs.Matches(PlayerSettings.GetVirtualRealitySDKs(TargetGroup)))
                {
                    PlayerSettings.SetVirtualRealitySDKs(TargetGroup, vrSDKs);
                }
            }

            if (TargetGroup == BuildTargetGroup.WSA)
            {
                EditorUserBuildSettings.wsaBuildAndRunDeployTarget = WSABuildAndRunDeployTarget.LocalMachine;
                EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
                PlayerSettings.WSA.inputSource = PlayerSettings.WSAInputSource.IndependentInputSource;
                WSASubtarget sub;
                if (Enum.TryParse(wsaSubtarget, out sub))
                {
                    EditorUserBuildSettings.wsaSubtarget = sub;
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, sub == WSASubtarget.HoloLens);
                }
            }
            else if (TargetGroup == BuildTargetGroup.Android)
            {
                AndroidSdkVersions sdkVersion;
                if (Enum.TryParse(androidSdkVersion, out sdkVersion))
                {
                    PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)Math.Max(
                        (int)PlayerSettings.Android.minSdkVersion,
                        (int)sdkVersion);
                }
            }
            else if (TargetGroup == BuildTargetGroup.iOS)
            {
                Version v;
                if (Version.TryParse(iOSVersion, out v)
                    && PlayerSettingsExt.iOS.TargetOSVersion < v)
                {
                    PlayerSettingsExt.iOS.TargetOSVersion = v;
                }
            }
        }

        public void UninstallAssetStorePackages(IProgress prog)
        {
            Platforms.ForEachPackage(UninstallableAssetStorePackages, prog, (pkg, p) => pkg.Uninstall(p));
        }

        public void UninstallZipPackages(IProgress prog)
        {
            Platforms.ForEachPackage(UninstallableZipPackages, prog, (pkg, p) => pkg.Uninstall(p));
        }

        public void UninstallUnityPackages(IProgress prog)
        {
            Platforms.ForEachPackage(UninstallableUnityPackages, prog, (pkg, p) => pkg.Uninstall(p));
        }

        public List<string> CompilerDefines
        {
            get
            {
                var defines = (from pkg in IncludedUnityPackages
                               where pkg.version != "exclude"
                               select (AbstractPackage)pkg)
                    .Union(ZipPackages)
                    .Union(AssetStorePackages)
                    .Select(pkg => pkg.CompilerDefine)
                    .Where(def => !string.IsNullOrEmpty(def))
                    .Distinct()
                    .ToList();

                if (!string.IsNullOrEmpty(CompilerDefine))
                {
                    defines.MaybeAdd(CompilerDefine);
                }

                if (TargetGroup == BuildTargetGroup.Android)
                {
                    for (var i = (int)PlayerSettings.Android.minSdkVersion; i > 0; --i)
                    {
                        if (Enum.IsDefined(typeof(AndroidSdkVersions), i))
                        {
                            defines.MaybeAdd("ANDROID_API_" + i);
                            defines.MaybeAdd("ANDROID_API_" + i + "_OR_GREATER");
                        }
                    }
                }
                else if (TargetGroup == BuildTargetGroup.iOS)
                {
                    for (var i = PlayerSettingsExt.iOS.TargetOSVersion.Major; i > 0; --i)
                    {
                        defines.MaybeAdd("IOS_VERSION_" + i);
                        defines.MaybeAdd("IOS_VERSION_" + i + "_OR_GREATER");
                    }
                }

                return defines;
            }
        }

        internal void Deactivate(PlatformConfiguration nextConfiguration)
        {
            AudioSettings.SetSpatializerPluginName(null);
            AudioSettingsExt.SetAmbisonicDecoderPluginName(null);
            PlayerSettings.virtualRealitySupported = false;
            PlayerSettings.Android.androidTVCompatibility = false;
            PlayerSettings.Android.ARCoreEnabled = false;
            PlayerSettingsExt.iOS.RequiresARKitSupport = false;
            PlayerSettings.vuforiaEnabled = false;
        }
    }
}
