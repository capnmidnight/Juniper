using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Juniper.Compression.Tar.GZip;
using Juniper.Json;
using Juniper.Progress;
using Juniper.XR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEditor;
using UnityEditor.Callbacks;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    /// <summary>
    /// An editor to respond to changes in XRSystem.
    /// </summary>
    public class JuniperConfigurationManager : EditorWindow
    {
        private const string MENU_NAME = "Juniper/";
        private static readonly ProjectConfiguration config;
        private static readonly Platforms platforms;

        private static readonly GUIContent TITLE = new GUIContent("Juniper");

        private static Vector2 packageScrollPosition;
        private static AssetStorePackage[] assetStorePackages;
        private static bool repaintNeeded;
        private static bool repaintBound;

        static JuniperConfigurationManager()
        {
            platforms = new Platforms();
            platforms.AssetStorePackagesUpdated += Platforms_PackagesUpdated;
            platforms.ScanningProgressUpdated += RepaintWindow;

            //platforms.StartFileWatcher();

            config = ProjectConfiguration.Load();
            config.PlatformChanged += Config_PlatformChanged;
            config.PlatformChangeConfirmed += StartBuild;

            if (RebuildNeeded)
            {
                OnEditorUpdate(() =>
                    NextPlatform = DesiredPlatform,
                    () => EditorUtility.DisplayDialog("Juniper", "Could not change platform.", "OK"));
            }
        }

        private static void Platforms_PackagesUpdated(AssetStorePackage[] packages)
        {
            assetStorePackages = packages;
            RepaintWindow();
        }

        private static void RepaintWindow()
        {
            repaintNeeded = true;
        }

        public void OnGUI()
        {
            titleContent = TITLE;

            if (!repaintBound)
            {
                repaintBound = true;
                EditorApplication.update += () =>
                {
                    if (repaintNeeded)
                    {
                        repaintNeeded = false;
                        Repaint();
                    }
                };


                var token = UnityEditorInternal.InternalEditorUtility.GetAuthToken();
                Task.Run(async () => {
                    var req = new UnityAssetStore.Requester(new JsonFactory());
                    const string UnityAssetStoreToken = "26c4202eb475d02864b40827dfff11a14657aa41";
                    const string UnityAssetStoreRoot = "https://www.assetstore.unity3d.com/";
                    const string userName = "sean.mcbeth@gmail.com";
                    const string password = "RzKuj0fd9f";
                    try
                    {
                        var sessionID = await req.Post($"{UnityAssetStoreRoot}login?skip_terms=1", $"user={userName}&pass={password}", UnityAssetStoreToken + token);
                        Debug.Log(sessionID);
                    }
                    catch//(Exception exp)
                    {

                    }
                    //req.GetDownloads("sean.mcbeth@gmail.com", "RzKuj0fd9f)
                    //    .ContinueWith(task =>
                    //    {
                    //        if (task.IsFaulted)
                    //        {
                    //            Debug.LogError(task.Exception);
                    //        }
                    //        else
                    //        {
                    //            foreach (var download in task.Result)
                    //            {
                    //                Debug.Log($"{download.name} ({download.id})");
                    //            }
                    //        }
                    //    });
                });
            }

            this.HeaderIndent("Status", () =>
            {
                this.Labeled("Build step", () => EditorGUILayout.LabelField(BuildStepName));
            });

            this.HeaderIndent("Platform", () =>
            {
                this.Labeled("Current Platform", () => EditorGUILayout.LabelField(CurrentPlatform.ToString()));
                this.Labeled("Desired Platform", () => EditorGUILayout.DropdownButton(new GUIContent(DesiredPlatform.ToString(), "Select the desired build platform"), FocusType.Keyboard));
            });

            /*
            this.HeaderIndent("Packages", () =>
            {
                if (assetStorePackages == null || assetStorePackages.Length == 0)
                {
                    EditorGUILayout.LabelField("(Loading)", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    packageScrollPosition = EditorGUILayout.BeginScrollView(packageScrollPosition);
                    foreach (var package in assetStorePackages)
                    {
                        this.HGroup(() =>
                        {
                            try
                            {
                                EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(package.Name));

                                if (package.ScanningProgress == AssetStorePackage.Status.None)
                                {
                                    EditorGUILayout.LabelField("Identified", EditorStyles.centeredGreyMiniLabel);
                                }
                                else if (package.ScanningProgress == AssetStorePackage.Status.Found
                                    || package.ScanningProgress == AssetStorePackage.Status.List)
                                {
                                    EditorGUILayout.LabelField("Found", EditorStyles.centeredGreyMiniLabel);
                                }
                                else if (package.ScanningProgress == AssetStorePackage.Status.NotFound)
                                {
                                    EditorGUILayout.LabelField("Not Found!");
                                }
                                else if (package.ScanningProgress == AssetStorePackage.Status.Listing)
                                {
                                    EditorGUILayout.LabelField("Listing", EditorStyles.centeredGreyMiniLabel);
                                }
                                else if (package.ScanningProgress == AssetStorePackage.Status.Listed
                                    || package.ScanningProgress == AssetStorePackage.Status.Scan
                                    || package.ScanningProgress == AssetStorePackage.Status.Scanning)
                                {
                                    EditorGUILayout.LabelField(string.Format(
                                        "({0} files)",
                                        package.TotalFiles),
                                        EditorStyles.centeredGreyMiniLabel);
                                    EditorGUILayout.LabelField("Scanning", EditorStyles.centeredGreyMiniLabel);
                                }
                                else if (package.ScanningProgress == AssetStorePackage.Status.Scanned)
                                {
                                    EditorGUILayout.LabelField(string.Format(
                                        "({0} of {1} files)",
                                        Units.Converter.Label(package.InstallPercentage, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent),
                                        package.TotalFiles),
                                        EditorStyles.centeredGreyMiniLabel);

                                    if (package.InstallPercentage < 1 && GUILayout.Button("Install"))
                                    {
                                        package.Install();
                                    }

                                    if (package.InstallPercentage > 0 && GUILayout.Button("Uninstall"))
                                    {
                                        package.Uninstall();
                                    }
                                }
                                else if (package.ScanningProgress == AssetStorePackage.Status.Error)
                                {
                                    EditorGUILayout.LabelField("ERROR! " + package.ErrorMessage, EditorStyles.miniBoldLabel);
                                }
                            }
                            catch
                            {

                            }
                        });
                    }
                    EditorGUILayout.EndScrollView();
                }
            });
            */
        }

        private static string BuildStepName
        {
            get
            {
                if (0 <= BuildProgress && BuildProgress < STAGES.Length)
                {
                    return STAGES[BuildProgress].Method.Name;
                }
                else
                {
                    return "Complete";
                }
            }
        }

        private static int BuildProgress
        {
            get
            {
                return config.BuildStep;
            }

            set
            {
                config.BuildStep = value;
            }
        }

        private static PlatformTypes CurrentPlatform
        {
            get
            {
                return JuniperPlatform.CurrentPlatform;
            }
        }

        private static PlatformTypes DesiredPlatform
        {
            get
            {
                return config.CurrentPlatform;
            }
        }

        private static bool BuildInProgress
        {
            get
            {
                return 0 <= BuildProgress && BuildProgress < STAGES.Length;
            }
        }

        private static bool RebuildNeeded
        {
            get
            {
                return DesiredPlatform != CurrentPlatform && !BuildInProgress;
            }
        }

        private static PlatformTypes NextPlatform
        {
            get
            {
                return config.NextPlatform;
            }

            set
            {
                config.NextPlatform = value;
            }
        }

        private static bool Config_PlatformChanged()
        {
            if (!platforms.PlatformDB[config.NextPlatform].IsSupported)
            {
                EditorUtility.DisplayDialog(
                   "Juniper",
                   $"The platform {config.NextPlatform} is not supported on your current system.",
                   "OK");
                return false;
            }
            else
            {
                return config.NextPlatform == PlatformTypes.None
                    || (config.NextPlatform != CurrentPlatform
                        && EditorUtility.DisplayDialog(
                            "Juniper",
                            $"You are about to change the platform from {CurrentPlatform} to {config.NextPlatform}. Do you want to continue?",
                            "Change platform",
                            "Don't change platform"));
            }
        }

        private static PlatformConfiguration LastConfiguration
        {
            get
            {
                return platforms.PlatformDB.Get(CurrentPlatform);
            }
        }

        private static PlatformConfiguration NextConfiguration
        {
            get
            {
                return platforms.PlatformDB.Get(NextPlatform);
            }
        }

        /// <summary>
        /// The DEFINE symbols that we use in this project. We need to filter them out of the whole
        /// set so we can add the right ones back in without deleting any that we don't control.
        /// </summary>
        private static string[] JUNIPER_DEFINES
        {
            get
            {
                var d = platforms.AllCompilerDefines.ToList();
                d.Add(RECOMPILE_SLUG);
                return d.Distinct().ToArray();
            }
        }

        #region Menu

        private static bool MenuCheck(PlatformTypes p)
        {
            return CurrentPlatform != p && platforms.PlatformDB[p].IsSupported;
        }

        [MenuItem(MENU_NAME + "Configuration Manager")]
        public static void ShowJuniperWindow()
        {
            EditorWindow.GetWindow<JuniperConfigurationManager>();
        }

        #region Menu/Android

        private const string ANDROID_MENU_NAME = MENU_NAME + "Android/";

        [MenuItem(ANDROID_MENU_NAME + "None", true)]
        public static bool SetAndroid_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.Android);
        }

        [MenuItem(ANDROID_MENU_NAME + "None", false)]
        public static void SetAndroid_MenuItem()
        {
            NextPlatform = PlatformTypes.Android;
        }

        [MenuItem(ANDROID_MENU_NAME + "ARCore", true)]
        public static bool SetAndroid_ARCore_MenuItem_Validate()
        {
#if UNITY_2018_2_OR_NEWER
            return MenuCheck(PlatformTypes.AndroidARCore);
#else
            return false;
#endif
        }

        [MenuItem(ANDROID_MENU_NAME + "ARCore", false)]
        public static void SetAndroid_ARCore_MenuItem()
        {
            NextPlatform = PlatformTypes.AndroidARCore;
        }

        [MenuItem(ANDROID_MENU_NAME + "Cardboard", true)]
        public static bool SetAndroid_Cardboard_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.AndroidCardboard);
        }

        [MenuItem(ANDROID_MENU_NAME + "Cardboard", false)]
        public static void SetAndroid_Cardboard_MenuItem()
        {
            NextPlatform = PlatformTypes.AndroidCardboard;
        }

        [MenuItem(ANDROID_MENU_NAME + "Daydream", true)]
        public static bool SetAndroid_Daydream_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.AndroidDaydream);
        }

        [MenuItem(ANDROID_MENU_NAME + "Daydream", false)]
        public static void SetAndroid_Daydream_MenuItem()
        {
            NextPlatform = PlatformTypes.AndroidDaydream;
        }

        [MenuItem(ANDROID_MENU_NAME + "Oculus", true)]
        public static bool SetAndroid_Oculus_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.AndroidOculus);
        }

        [MenuItem(ANDROID_MENU_NAME + "Oculus", false)]
        public static void SetAndroid_Oculus_MenuItem()
        {
            NextPlatform = PlatformTypes.AndroidOculus;
        }

        [MenuItem(ANDROID_MENU_NAME + "Pico G2", true)]
        public static bool SetAndroid_PicoG2_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.AndroidPicoG2);
        }

        [MenuItem(ANDROID_MENU_NAME + "Pico G2", false)]
        public static void SetAndroid_PicoG2_MenuItem()
        {
            NextPlatform = PlatformTypes.AndroidPicoG2;
        }

        [MenuItem(ANDROID_MENU_NAME + "Vive Focus", true)]
        public static bool SetAndroid_ViveFocus_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.AndroidViveFocus);
        }

        [MenuItem(ANDROID_MENU_NAME + "Vive Focus", false)]
        public static void SetAndroid_ViveFocus_MenuItem()
        {
            NextPlatform = PlatformTypes.AndroidViveFocus;
        }

        #endregion Menu/Android

        #region Menu/IOS

        private const string IOS_MENU_NAME = MENU_NAME + "IOS/";

        [MenuItem(IOS_MENU_NAME + "None", true)]
        public static bool SetIOS_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.IOS);
        }

        [MenuItem(IOS_MENU_NAME + "None", false)]
        public static void SetIOS_MenuItem()
        {
            NextPlatform = PlatformTypes.IOS;
        }

        [MenuItem(IOS_MENU_NAME + "ARKit", true)]
        public static bool SetIOS_ARKit_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.IOSARKit);
        }

        [MenuItem(IOS_MENU_NAME + "ARKit", false)]
        public static void SetIOS_ARKit_MenuItem()
        {
            NextPlatform = PlatformTypes.IOSARKit;
        }

        [MenuItem(IOS_MENU_NAME + "Cardboard", true)]
        public static bool SetIOS_Cardboard_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.IOSCardboard);
        }

        [MenuItem(IOS_MENU_NAME + "Cardboard", false)]
        public static void SetIOS_Cardboard_MenuItem()
        {
            NextPlatform = PlatformTypes.IOSCardboard;
        }

        #endregion Menu/IOS

        #region Menu/Standalone

        private const string STANDALONE_MENU_NAME = MENU_NAME + "Standalone/";

        [MenuItem(STANDALONE_MENU_NAME + "None", true)]
        public static bool SetStandalone_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.Standalone);
        }

        [MenuItem(STANDALONE_MENU_NAME + "None", false)]
        public static void SetStandalone_MenuItem()
        {
            NextPlatform = PlatformTypes.Standalone;
        }

        [MenuItem(STANDALONE_MENU_NAME + "Oculus", true)]
        public static bool SetStandalone_Oculus_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.StandaloneOculus);
        }

        [MenuItem(STANDALONE_MENU_NAME + "Oculus", false)]
        public static void SetStandalone_Oculus_MenuItem()
        {
            NextPlatform = PlatformTypes.StandaloneOculus;
        }

        [MenuItem(STANDALONE_MENU_NAME + "SteamVR", true)]
        public static bool SetStandalone_SteamVR_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.StandaloneSteamVR);
        }

        [MenuItem(STANDALONE_MENU_NAME + "SteamVR", false)]
        public static void SetStandalone_SteamVR_MenuItem()
        {
            NextPlatform = PlatformTypes.StandaloneSteamVR;
        }

        #endregion Menu/Standalone

        #region Menu/UWP

        private const string UWP_MENU_NAME = MENU_NAME + "UWP/";

        [MenuItem(UWP_MENU_NAME + "None", true)]
        public static bool SetUWP_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.UWP);
        }

        [MenuItem(UWP_MENU_NAME + "None", false)]
        public static void SetUWP_MenuItem()
        {
            NextPlatform = PlatformTypes.UWP;
        }

        [MenuItem(UWP_MENU_NAME + "WindowsMR", true)]
        public static bool SetUWP_WindowsMR_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.UWPWindowsMR);
        }

        [MenuItem(UWP_MENU_NAME + "WindowsMR", false)]
        public static void SetUWP_WindowsMR_MenuItem()
        {
            NextPlatform = PlatformTypes.UWPWindowsMR;
        }

        [MenuItem(UWP_MENU_NAME + "HoloLens", true)]
        public static bool SetUWP_HoloLens_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.UWPHoloLens);
        }

        [MenuItem(UWP_MENU_NAME + "HoloLens", false)]
        public static void SetUWP_HoloLens_MenuItem()
        {
            NextPlatform = PlatformTypes.UWPHoloLens;
        }

        #endregion Menu/UWP

        #region Menu/LuminOS

        [MenuItem(MENU_NAME + "Magic Leap", true)]
        public static bool SetMagicLeap_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.MagicLeap);
        }

        [MenuItem(MENU_NAME + "Magic Leap", false)]
        public static void SetMagicLeap_MenuItem()
        {
            NextPlatform = PlatformTypes.MagicLeap;
        }

        #endregion Menu/LuminOS

        #region Menu/WebGL

        [MenuItem(MENU_NAME + "WebGL", true)]
        public static bool SetWebGL_MenuItem_Validate()
        {
            return MenuCheck(PlatformTypes.WebGL);
        }

        [MenuItem(MENU_NAME + "WebGL", false)]
        public static void SetWebGL_MenuItem()
        {
            NextPlatform = PlatformTypes.WebGL;
        }

        #endregion

        #region Menu/Other

        private const string OTHER_MENU_NAME = MENU_NAME + "Other/";

        [MenuItem(OTHER_MENU_NAME + "Clear Errant Progress Dialogs", false, 202)]
        public static void ClearProgressDialog_MenuItem()
        {
            EditorUtility.ClearProgressBar();
        }

        [MenuItem(OTHER_MENU_NAME + "Reset configuration", false, 203)]
        public static void Other_ResetConfiguration()
        {
            StartBuild();
        }

        [MenuItem(OTHER_MENU_NAME + "Resume build", true)]
        public static bool Other_ResumeBuild_Validate()
        {
            return BuildInProgress;
        }

        [MenuItem(OTHER_MENU_NAME + "Resume build", false, 204)]
        public static void Other_ResumeBuild()
        {
            ResumeBuild();
        }

        #endregion Menu/Other

        #endregion Menu

        private const string RECOMPILE_SLUG = "RECOMPILE_SLUG";

        private static void OnCancel()
        {
            OnCancel(true);
        }

        private static void OnCancel(bool canceledByUser)
        {
            if (canceledByUser)
            {
                Debug.LogWarning("Canceled by user");
            }
            EditorUtility.ClearProgressBar();
            BuildProgress = -1;
        }

        private static void OnEditorUpdate(Action resolve, Action reject = null, Func<bool> test = null)
        {
            if (test == null)
            {
                test = () => true;
            }

            void exec()
            {
                if (test())
                {
                    try
                    {
                        EditorApplication.update -= exec;
                        resolve();
                    }
                    catch (OperationCanceledException)
                    {
                        EditorApplication.update -= exec;
                        reject();
                    }
                }
            }

            EditorApplication.update += exec;
        }

        private static void DelayedUpdate(IProgress prog, float maxTime, Action onComplete)
        {
            prog?.Report(0);
            var startTime = DateTime.Now;
            OnEditorUpdate(() =>
            {
                EditorUtility.ClearProgressBar();
                onComplete();
            }, OnCancel, () =>
            {
                var age = (float)(DateTime.Now - startTime).TotalSeconds;
                prog?.Report(age, maxTime);
                return age > maxTime;
            });
        }

        private static List<string> CleanupDefines(IEnumerable<string> defs)
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

        private static string LastPrefix;

        private static IProgress PrepareBuildStep(int offset, string prefix = null)
        {
            LastPrefix = prefix ?? LastPrefix;
            var step = BuildProgress - offset + 1;
            var msg = $"Juniper ({step} of {STAGES.Length}): {LastPrefix}";
            return new UnityEditorProgressDialog(msg)
                .Subdivide(step + BuildProgress, 2 * STAGES.Length, LastPrefix);
        }

        private static void WithProgress(string prefix, Action<IProgress> act)
        {
            try
            {
                Debug.Log("Juniper: " + prefix);
                var prog = PrepareBuildStep(0, prefix);
                prog.Report(0);
                act(prog);
            }
            catch (OperationCanceledException)
            {
                OnCancel();
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
            }
        }

        private static void Recompile(bool advanceBuildStep, params PlatformConfiguration[] platforms)
        {
            var prog = PrepareBuildStep(1);

            InternalCompile(advanceBuildStep, platforms);

            DelayedUpdate(prog, 30, () => Debug.LogWarning("Timeout!!!"));
        }

        private static void InternalCompile(bool advanceBuildStep, params PlatformConfiguration[] platforms)
        {
            if (advanceBuildStep)
            {
                ++BuildProgress;
            }

            var commonDefines = platforms
                .SelectMany(p => p.CompilerDefines)
                .GroupBy(d => d)
                .Where(defs => defs.Count() == platforms.Length)
                .SelectMany(defs => defs)
                .Distinct()
                .ToList();

            var currentDefines = CleanupDefines(PlayerSettings
                .GetScriptingDefineSymbolsForGroup(platforms[0].TargetGroup)
                .Split(';'))
                .ToList();

            var nonJuniper = currentDefines
                .Exclude(JUNIPER_DEFINES)
                .Where(def => !def.StartsWith("NO_UNITY"))
                .Distinct()
                .ToList();

            var nextDefines = nonJuniper
                .Union(commonDefines)
                .ToList();

            nextDefines = CleanupDefines(nextDefines);

            if (nextDefines.Matches(currentDefines))
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

            var nextDefinesString = string.Join(";", nextDefines);

            var targetGroups = platforms
                .Select(p => p.TargetGroup)
                .Distinct()
                .Reverse()
                .ToArray();

            foreach (var targetGroup in targetGroups)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, nextDefinesString);
            }
        }

        private static readonly Action[] STAGES =
        {
            DeactivatePlatform,
            RefreshPackages,
            PrepareProject
        };

        [DidReloadScripts]
        private static void ResumeBuild()
        {
            if (BuildInProgress)
            {
                STAGES[BuildProgress].Invoke();
            }
        }

        private static readonly string MANIFEST_FILE = PathExt.FixPath(
#if UNITY_2018_1_OR_NEWER
            "Packages/manifest.json"
#else
            "UnityPackageManager/manifest.json"
#endif
        );

        private static void StartBuild()
        {
            BuildProgress = 0;
            DeactivatePlatform();
        }

        private static void DeactivatePlatform()
        {
            WithProgress("Resetting to base configuration", _ =>
            {
                JuniperPlatform.Uninstall();
                LastConfiguration.Deactivate(NextConfiguration);
                Recompile(true, LastConfiguration, NextConfiguration);
            });
        }

        public static void RefreshPackages()
        {
            WithProgress("Refreshing packages " + NextPlatform, prog =>
            {
                var progs = prog.Split(4);
                LastConfiguration.UninstallRawPackages(progs[0]);

                var manifest = JObject.Parse(File.ReadAllText(MANIFEST_FILE));
                var deps = (JObject)manifest["dependencies"];
                if (deps == null)
                {
                    deps = new JObject();
                    manifest.Add("dependencies", deps);
                }

                UnityPackage.Dependencies = deps;
                LastConfiguration.UninstallUnityPackages(progs[1]);
                NextConfiguration.InstallUnityPackages(progs[2]);

                var txt = manifest.ToString(Formatting.Indented);
                FileExt.WriteAllText(MANIFEST_FILE, txt);

                NextConfiguration.InstallRawPackages(progs[3]);

                if (NextConfiguration.TargetSwitchNeeded)
                {
                    InternalCompile(false, NextConfiguration);
                    NextConfiguration.SwitchTarget();
                }
                else
                {
                    InternalCompile(true, NextConfiguration);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
                    if (!EditorApplication.isCompiling)
                    {
                        ResumeBuild();
                    }
                }
            });
        }

        public static void PrepareProject()
        {
            WithProgress("Preparing project", prog =>
            {
                NextConfiguration.Activate(prog);
                JuniperPlatform.Install(true);
                config.Commit();
                OnCancel(false);
            });
        }
    }
}
