using Juniper.Progress;
using Juniper.Unity.Input.Speech;
using Juniper.Unity.World;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Juniper.UnityEditor.ConfigurationManagement
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
                return (BuildTarget)Enum.Parse(typeof(BuildTarget), localBuildTarget);
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
#if UNITY_2018_1_OR_NEWER
                return BuildPipeline.IsBuildTargetSupported(TargetGroup, BuildTarget);
#else
                var moduleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
                var isPlatformSupportLoaded = moduleManager.GetMethod(
                    "IsPlatformSupportLoaded",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                var getTargetStringFromBuildTarget = moduleManager.GetMethod(
                    "GetTargetStringFromBuildTarget",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                var targetString = (string)getTargetStringFromBuildTarget.Invoke(null, new object[] {BuildTarget.tvOS});
                return (bool)isPlatformSupportLoaded.Invoke(null,new object[] {targetString});
#endif
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
        public RawPackage[] UninstallableRawPackages
        {
            get; internal set;
        }

        [JsonIgnore]
        public RawPackage[] RawPackages
        {
            get; internal set;
        }

        public bool SwitchTarget()
        {
            if (BuildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                Debug.Log($"Switching build target from {EditorUserBuildSettings.activeBuildTarget} to {BuildTarget}.");
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(TargetGroup, BuildTarget);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void InstallUnityPackages(IProgressReceiver prog = null)
        {
            var progs = prog.Split(2);
            Platforms.ForEachPackage(ExcludedUnityPackages, progs[0], (pkg, p) =>
#if UNITY_2018_2_OR_NEWER
                pkg.Uninstall(p)
#else
                pkg.Install(p)
#endif
            );
            Platforms.ForEachPackage(IncludedUnityPackages, progs[1], (pkg, p) =>
            {
#if UNITY_2018_2_OR_NEWER
                if (pkg.Name != "com.unity.xr.magicleap")
                {
                    pkg.Install(p);
                }
#else
                if (pkg.Name != "com.unity.xr.magicleap" && !pkg.Name.StartsWith("com.unity.modules."))
                {
                    pkg.Install(p);
                }
#endif
            });
        }

        public void InstallRawPackages(IProgressReceiver prog = null)
        {
            Platforms.ForEachPackage(RawPackages, prog, (pkg, p) => pkg.Install(p));
        }

        public void Activate(IProgressReceiver prog)
        {
            var progs = prog.Split(2);
            Platforms.ForEachPackage(IncludedUnityPackages, progs[0], (pkg, p) => pkg.Activate(TargetGroup, p));
            Platforms.ForEachPackage(RawPackages, progs[1], (pkg, p) => pkg.Activate(TargetGroup, p));

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
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.WebCam, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Bluetooth, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, ComponentExt.FindAny<KeywordRecognizer>() != null);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Location, ComponentExt.FindAny<GPSLocation>() != null);
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

        public void UninstallRawPackages(IProgressReceiver prog = null)
        {
            Platforms.ForEachPackage(UninstallableRawPackages, prog, (pkg, p) => pkg.Uninstall(p));
        }

        public void UninstallUnityPackages(IProgressReceiver prog = null)
        {
            Platforms.ForEachPackage(UninstallableUnityPackages, prog, (pkg, p) => pkg.Uninstall(p));
        }

        public List<string> CompilerDefines
        {
            get
            {
                var defines = Platforms.GetCompilerDefines(IncludedUnityPackages, RawPackages).ToList();

                if (!string.IsNullOrEmpty(CompilerDefine))
                {
                    defines.Add(CompilerDefine);
                }

                if (TargetGroup == BuildTargetGroup.Android)
                {
                    for (var i = (int)PlayerSettings.Android.minSdkVersion; i > 0; --i)
                    {
                        if (Enum.IsDefined(typeof(AndroidSdkVersions), i))
                        {
                            defines.Add("ANDROID_API_" + i);
                            defines.Add("ANDROID_API_" + i + "_OR_GREATER");
                        }
                    }
                }
                else if (TargetGroup == BuildTargetGroup.iOS)
                {
                    for (var i = PlayerSettingsExt.iOS.TargetOSVersion.Major; i > 0; --i)
                    {
                        defines.Add("IOS_VERSION_" + i);
                        defines.Add("IOS_VERSION_" + i + "_OR_GREATER");
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
#if UNITY_2018_2_OR_NEWER
            PlayerSettings.Android.ARCoreEnabled = false;
#endif
            PlayerSettingsExt.iOS.RequiresARKitSupport = false;
            PlayerSettings.SetPlatformVuforiaEnabled(TargetGroup, false);
            if (nextConfiguration.TargetGroup != TargetGroup)
            {
                PlayerSettings.SetPlatformVuforiaEnabled(TargetGroup, false);
            }
        }
    }
}
