using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.IO;

using UnityEditor;

using UnityEngine;

using static System.Console;

namespace Juniper.ConfigurationManagement
{
    public static class Program
    {
        public static void Main()
        {
            Project.UnityProjectRoot = @"D:\Projects\Juniper\examples\Juniper - Android";
            var packageDB = AbstractPackage.Load();
            var manifest = UnityPackageManifest.Load();
            var platforms = Platforms.Load();

            foreach (var package in platforms.Packages)
            {
                PrintPackageOps(packageDB, manifest, package);
            }


            foreach (var configuration in platforms.Configurations)
            {
                WriteLine("============================");
                WriteLine(configuration.Name);
                foreach (var package in configuration.Packages)
                {
                    PrintPackageOps(packageDB, manifest, package);
                }
            }
        }

        private static void PrintPackageOps(IReadOnlyDictionary<string, IReadOnlyCollection<AbstractPackage>> packageDB, UnityPackageManifest manifest, PackageRequirement req)
        {
            WriteLine(req);
            if (req.ForRemoval)
            {
                Write("Removing... ");
                if (!manifest.ContainsKey(req.PackageID))
                {
                    WriteLine("no need to remove, not in manifest.");
                }
                else
                {
                    WriteLine(manifest[req.PackageID]);
                }
            }
            else
            {
                Write("Adding... ");
                if (!packageDB.ContainsKey(req.PackageID))
                {
                    WriteLine("no package of any version found!");
                }
                else
                {
                    var match = packageDB[req.PackageID]
                        .OrderByDescending(p => p.CompareTo(req))
                        .FirstOrDefault();

                    if (match == req)
                    {
                        WriteLine(match);
                    }
                    else if (match is null)
                    {
                        WriteLine("couldn't find a matching version of the package.");
                    }
                    else if (match < req)
                    {
                        WriteLine("couldn't find the package, but an older version already exists in the manifest: " + match);
                    }
                    else
                    {
                        WriteLine("couldn't find the package, but an newer version already exists in the manifest: " + match);
                    }
                }

                if (req.Source == PackageSources.UnityPackageManager
                    && manifest.ContainsKey(req.PackageID))
                {
                    Write("The package exists in the manifest ");

                    var match = manifest[req.PackageID];
                    if (req == match)
                    {
                        WriteLine("and is an exact match.");
                    }
                    else if (match < req)
                    {
                        WriteLine(" but the version in the manifest is older: " + match);
                    }
                    else
                    {
                        WriteLine(" but the version in the manifest is newer: " + match);
                    }
                }
            }

            WriteLine();
        }

        public static BuildTarget GetBuildTarget(PlatformConfiguration config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var localBuildTarget = config.BuildTarget;
            if (localBuildTarget == "Standalone")
            {
#if UNITY_EDITOR_WIN
                if (Environment.Is64BitOperatingSystem)
                {
                    localBuildTarget = nameof(UnityEditor.BuildTarget.StandaloneWindows64);
                }
                else
                {
                    localBuildTarget = nameof(UnityEditor.BuildTarget.StandaloneWindows);
                }
#elif UNITY_EDITOR_OSX
                localBuildTarget = nameof(UnityEditor.BuildTarget.StandaloneOSX);
#else
                localBuildTarget = nameof(UnityEditor.BuildTarget.StandaloneLinux64);
#endif
            }

            try
            {
                return (BuildTarget)Enum.Parse(typeof(BuildTarget), localBuildTarget);
            }
            catch
            {
                return UnityEditor.BuildTarget.NoTarget;
            }
        }

        public static BuildTargetGroup GetTargetGroup(PlatformConfiguration config)
        {
            return BuildPipeline.GetBuildTargetGroup(GetBuildTarget(config));
        }

        public static bool IsSupported(PlatformConfiguration config)
        {
            return BuildPipeline.IsBuildTargetSupported(GetTargetGroup(config), GetBuildTarget(config));
        }

        public static void SwitchTarget(PlatformConfiguration config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Debug.Log($"Switching build target from {EditorUserBuildSettings.activeBuildTarget} to {config.BuildTarget}.");
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(GetTargetGroup(config), GetBuildTarget(config));
        }

        public static bool TargetSwitchNeeded(PlatformConfiguration config)
        {
            return GetBuildTarget(config) != EditorUserBuildSettings.activeBuildTarget;
        }


        public static void Activate(PlatformConfiguration config, IProgress prog)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var progs = prog.Split(2);

            if (!string.IsNullOrEmpty(config.Spatializer))
            {
                var hasSpatializer = AudioSettings
                    .GetSpatializerPluginNames()
                    .Contains(config.Spatializer);

                if (hasSpatializer)
                {
                    AudioSettings.SetSpatializerPluginName(config.Spatializer);
                    AudioSettingsExt.SetAmbisonicDecoderPluginName(config.Spatializer);
                }
            }

            var buildTargetGroup = GetTargetGroup(config);
            var supportedVRSDKs = PlayerSettings.GetAvailableVirtualRealitySDKs(buildTargetGroup);
            var vrSDKs = config
                .XRPlatforms
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
                if (Enum.TryParse(config.WsaSubtarget, out WSASubtarget sub))
                {
                    EditorUserBuildSettings.wsaSubtarget = sub;
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, sub == WSASubtarget.HoloLens);
                }
            }
            else if (buildTargetGroup == BuildTargetGroup.Android
                && Enum.TryParse(config.AndroidSdkVersion, out AndroidSdkVersions sdkVersion))
            {
                PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)Math.Max(
                    (int)PlayerSettings.Android.minSdkVersion,
                    (int)sdkVersion);
            }
            else if (buildTargetGroup == BuildTargetGroup.iOS
                && Version.TryParse(config.IOSVersion, out var v)
                && ApplleiOS.TargetOSVersion < v)
            {
                ApplleiOS.TargetOSVersion = v;
            }
        }

        public static void Activate(JuniperPackage package, BuildTargetGroup targetGroup)
        {
            if (package is null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (package.Name == "Vuforia")
            {
                PlayerSettings.vuforiaEnabled = true;
            }

            if (package is null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (targetGroup == BuildTargetGroup.Android)
            {
                if (package.Name == "GoogleARCore")
                {
                    PlayerSettings.Android.ARCoreEnabled = true;
                }
                else if (package.Name == "GoogleVR")
                {
                    FileExt.Copy(
                        PathExt.FixPath("Assets/GoogleVR/Plugins/Android/AndroidManifest-6DOF.xml"),
                        PathExt.FixPath("Assets/Plugins/Android/AndroidManifest.xml"),
                        true);
                }
            }
            else if (targetGroup == BuildTargetGroup.iOS)
            {
                if (package.Name == "UnityARKitPlugin")
                {
                    Juniper.ApplleiOS.RequiresARKitSupport = true;

                    if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    {
                        PlayerSettings.iOS.cameraUsageDescription = "Augmented reality camera view";
                    }
                }
            }
        }

        public static List<string> GetCompilerDefines(PlatformConfiguration config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var defines = new List<string>();

            if (!string.IsNullOrEmpty(config.CompilerDefine))
            {
                defines.MaybeAdd(config.CompilerDefine);
            }

            if (GetTargetGroup(config) == BuildTargetGroup.Android)
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
            else if (GetTargetGroup(config) == BuildTargetGroup.iOS)
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
