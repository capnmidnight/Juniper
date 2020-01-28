using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Juniper.XR;

using UnityEditor;

namespace Juniper.ConfigurationManagement
{
    public static class Project
    {
        private const string RECOMPILE_SLUG = "RECOMPILE_SLUG";

        private static string unityProjectDirectoryName;

        public static PlatformType Platform => ProjectConfiguration.Platform;

        private static IReadOnlyDictionary<string, IReadOnlyCollection<AbstractPackage>> packageDB;

        public static IReadOnlyDictionary<string, IReadOnlyCollection<AbstractPackage>> PackageDatabase
        {
            get
            {
                if (packageDB is null)
                {
                    packageDB = AbstractPackage.Load();
                }

                return packageDB;
            }
        }

        public static string UnityProjectRoot
        {
            get
            {
                if (unityProjectDirectoryName is null)
                {
                    unityProjectDirectoryName = Environment.CurrentDirectory;
                }

                return unityProjectDirectoryName;
            }
            set
            {
                unityProjectDirectoryName = value;
            }
        }

        private static string unityEditorDirectoryName;

        public static string UnityEditorRoot
        {
            get
            {
                if (unityEditorDirectoryName is null)
                {
                    unityEditorDirectoryName = Environment.GetEnvironmentVariable("UNITY_ROOT");
                    if (unityEditorDirectoryName is null)
                    {
                        var exe = new FileInfo(Assembly.GetEntryAssembly().Location);
                        if (exe.Name == "Unity.exe")
                        {
                            unityEditorDirectoryName = exe.Directory.Parent.FullName;
                        }
                    }
                }
                return unityEditorDirectoryName;
            }
        }

        public static string UnityAssetsPath => Path.Combine(UnityProjectRoot, "Assets");

        public static string PluginsPath => Path.Combine(UnityProjectRoot, "Plugins");

        public static string JuniperAssetPath => Path.Combine(UnityAssetsPath, "Juniper");

        public static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static readonly string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static readonly string UserDownloads = Path.Combine(UserProfile, "Downloads");

        public static List<string> CleanupDefines(IEnumerable<string> defs)
        {
            var defines = defs.Distinct().ToList();
            defines.RemoveAll(string.IsNullOrWhiteSpace);
            defines.Sort();

            // move the slug to the end, if it exists
            if (defines.Contains(RECOMPILE_SLUG))
            {
                defines.Remove(RECOMPILE_SLUG);
                defines.Add(RECOMPILE_SLUG);
            }

            return defines;
        }

        public static List<string> GetDefines(BuildTargetGroup targetGroup)
        {
            var defString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defArray = defString.SplitX(';');
            var defines = CleanupDefines(defArray);
            defines.Remove(RECOMPILE_SLUG);
            return defines;
        }

        public static void AddCompilerDefine(string compilerDefine)
        {
            var defines = GetDefines();
            defines.Add(compilerDefine);
            SetDefines(defines);
        }

        public static void RemoveCompilerDefine(string compilerDefine)
        {
            var defines = GetDefines();
            defines.Remove(compilerDefine);
            SetDefines(defines);
        }

        public static BuildTargetGroup CurrentBuildTargetGroup =>
            BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

        public static List<string> GetDefines()
        {
            return GetDefines(CurrentBuildTargetGroup);
        }

        public static void SetDefines(BuildTargetGroup targetGroup, List<string> nextDefines, bool forceRecompile)
        {
            nextDefines = CleanupDefines(nextDefines);
            var currentDefines = GetDefines(targetGroup);
            if (nextDefines.Matches(currentDefines) && forceRecompile)
            {
                if (nextDefines.Contains(RECOMPILE_SLUG))
                {
                    nextDefines.Remove(RECOMPILE_SLUG);
                }
                else
                {
                    nextDefines.Add(RECOMPILE_SLUG);
                }
            }

            if (!nextDefines.Matches(currentDefines))
            {
                var defString = string.Join(";", nextDefines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defString);
            }
        }

        public static void SetDefines(BuildTargetGroup targetGroup, List<string> nextDefines)
        {
            SetDefines(targetGroup, nextDefines, false);
        }

        public static void SetDefines(List<string> nextDefines, bool forceRecompile)
        {
            SetDefines(CurrentBuildTargetGroup, nextDefines, forceRecompile);
        }

        public static void SetDefines(List<string> nextDefines)
        {
            SetDefines(nextDefines, false);
        }
    }
}
