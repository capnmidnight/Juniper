using Juniper.Data;
using Juniper.Progress;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Juniper.Unity.ConfigurationManagement
{
    internal sealed class RawPackage : AbstractPackage
    {
        public static string ROOT_DIRECTORY = PathExt.FixPath("Assets/Juniper/ThirdParty/Optional");

        public string spatializer;
        public string androidSdkVersion;
        public string iOSVersion;
        public string wsaSubtarget;

        public RawPackage()
        {
            spatializer = null;
            androidSdkVersion = null;
            iOSVersion = null;
            wsaSubtarget = null;
        }

        [JsonIgnore]
        public string InputZipFileName
        {
            get
            {
                return Path.ChangeExtension(Path.Combine(ROOT_DIRECTORY, Name), "zip");
            }
        }

        public override void Install(IProgressReceiver prog = null)
        {
            base.Install(prog);

            if (File.Exists(InputZipFileName))
            {
                Zip.DecompressDirectory(InputZipFileName, "Assets", prog);
            }

            prog?.SetProgress(1);
        }

        public override void Activate(BuildTargetGroup targetGroup, IProgressReceiver prog = null)
        {
            base.Activate(targetGroup, prog);

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

            if (Name == "Vuforia")
            {
                PlayerSettings.SetPlatformVuforiaEnabled(targetGroup, true);
            }

            if (targetGroup == BuildTargetGroup.Android)
            {
                if (Name == "GoogleARCore")
                {
#if UNITY_2018_2_OR_NEWER
                    PlayerSettings.Android.ARCoreEnabled = true;
#endif
                }
                else if (Name == "GoogleVR")
                {
                    FileExt.Copy(
                        PathExt.FixPath("Assets/GoogleVR/Plugins/Android/AndroidManifest-6DOF.xml"),
                        PathExt.FixPath("Assets/Plugins/Android/AndroidManifest.xml"),
                        true);
                }

                AndroidSdkVersions sdkVersion;
                if (Enum.TryParse(androidSdkVersion, out sdkVersion))
                {
                    PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)Math.Max(
                        (int)PlayerSettings.Android.minSdkVersion,
                        (int)sdkVersion);
                }
            }
            else if (targetGroup == BuildTargetGroup.iOS)
            {
                Version v;
                if (Version.TryParse(iOSVersion, out v)
                    && PlayerSettingsExt.iOS.TargetOSVersion < v)
                {
                    PlayerSettingsExt.iOS.TargetOSVersion = v;
                }

                if (Name == "UnityARKitPlugin")
                {
                    PlayerSettingsExt.iOS.RequiresARKitSupport = true;

                    if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    {
                        PlayerSettings.iOS.cameraUsageDescription = "Augmented reality camera view";
                    }
                }
            }
            else if (targetGroup == BuildTargetGroup.WSA)
            {
                WSASubtarget sub;
                if (Enum.TryParse(wsaSubtarget, out sub))
                {
                    EditorUserBuildSettings.wsaSubtarget = sub;
                    PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, sub == WSASubtarget.HoloLens);
                }
            }
        }

        private static void DeleteAll(IEnumerable<string> paths, Func<string, bool> tryDelete, IProgressReceiver prog)
        {
            prog?.SetProgress(0, "Deleting");

            var prefixedPath = paths
                .Select(path => Path.Combine("Assets", path))
                .ToArray();

            var subProgs = prog.Split(prefixedPath.Length);
            var index = 0;
            foreach (var path in prefixedPath)
            {
                try
                {
                    subProgs[index]?.SetProgress(0);
                    if (tryDelete(path))
                    {
                        subProgs[index]?.SetProgress(1);
                    }
                    ++index;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);
                }
            }

            prog?.SetProgress(1, "Deleted");
        }

        public override void Uninstall(IProgressReceiver prog = null)
        {
            base.Uninstall(prog);

            var progs = prog.Split(4);

            var dirs = Zip.RecurseDirectories(InputZipFileName, progs[2]);
            var files = Zip.RecurseFiles(InputZipFileName, progs[0]);
            files = files
                .Union(files.Select(file => file + ".meta"))
                .Union(dirs.Select(dir => dir + ".meta"))
                .Where(path => !path.EndsWith(".meta.meta"));
            DeleteAll(files, FileExt.TryDelete, progs[1]);
            DeleteAll(dirs.Reverse(), DirectoryExt.TryDelete, progs[3]);
        }
    }
}
