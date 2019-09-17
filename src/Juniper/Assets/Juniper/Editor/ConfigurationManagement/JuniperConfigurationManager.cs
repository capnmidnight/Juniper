using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Json.Lite;
using Json.Lite.Linq;

using Juniper.Compression.Tar.GZip;
using Juniper.Progress;
using Juniper.XR;

using UnityEditor;
using UnityEditor.Callbacks;

using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    /// <summary>
    /// An editor to respond to changes in XRSystem.
    /// </summary>
    [SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0102:Non-overridden virtual method call on value type", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0202:Value type to reference type conversion allocation for string concatenation", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "<Pending>")]
    public class JuniperConfigurationManager : EditorWindow
    {
        private const string MENU_NAME = "Juniper/";
        private static readonly ProjectConfiguration config;
        private static readonly Platforms platforms;

        private static readonly GUIContent TITLE = new GUIContent("Juniper");

        private static string newDefine;
        private static Vector2 definesScrollPosition;
        private static Vector2 packageScrollPosition;
        private static AbstractFilePackage[] packages;
        private static bool repaintNeeded;
        private static bool repaintBound;

        static JuniperConfigurationManager()
        {
            platforms = new Platforms();
            platforms.PackagesUpdated += Platforms_PackagesUpdated;
            platforms.ScanningProgressUpdated += RepaintWindow;

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

        private static void Platforms_PackagesUpdated(AbstractFilePackage[] newPackages)
        {
            packages = newPackages;
            RepaintWindow();
        }

        private static void RepaintWindow()
        {
            repaintNeeded = true;
        }

        private void EditorUpdate()
        {
            if (repaintNeeded)
            {
                Repaint();
            }

            if (!platforms.IsRunning)
            {
                platforms.StartFileWatcher();
            }
        }

        private const float nameFieldWidth = 200;
        private const float narrowWidth = 50;
        private const float buttonWidth = 100;

        public void OnGUI()
        {
            titleContent = TITLE;

            if (!repaintBound)
            {
                repaintBound = true;
                EditorApplication.update += EditorUpdate;

            }

            repaintNeeded = false;

            var nameFieldGWidth = GUILayout.Width(nameFieldWidth);
            var narrowGWidth = GUILayout.Width(narrowWidth);
            var statusWidth = GUILayout.Width(120);
            var buttonGWidth = GUILayout.Width(buttonWidth);

            var selectedPlatform = CurrentPlatform;

            this.HeaderIndent("Status", () =>
            {
                this.HGroup(() =>
                {
                    EditorGUILayout.LabelField("Build step", nameFieldGWidth);
                    EditorGUILayout.LabelField(BuildStepName);
                });

                this.HGroup(() =>
                {
                    EditorGUILayout.LabelField("Platform", nameFieldGWidth);
                    selectedPlatform = (PlatformTypes)EditorGUILayout.EnumPopup(DesiredPlatform, nameFieldGWidth);
                });
            });

            if (selectedPlatform != CurrentPlatform)
            {
                if (MenuCheck(selectedPlatform))
                {
                    NextPlatform = DesiredPlatform;
                }
            }
            else
            {
                UpdateUnityPackages();

                this.HeaderIndent("Packages", () =>
                {
                    if (packages == null || packages.Length == 0)
                    {
                        EditorGUILayout.LabelField("(Loading)", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        this.HGroup(() =>
                        {
                            EditorGUILayout.LabelField("Name", EditorStyles.centeredGreyMiniLabel, nameFieldGWidth);
                            EditorGUILayout.LabelField("Define", EditorStyles.centeredGreyMiniLabel, nameFieldGWidth);
                            EditorGUILayout.LabelField("Required", EditorStyles.centeredGreyMiniLabel, narrowGWidth);
                            EditorGUILayout.LabelField("Status", EditorStyles.centeredGreyMiniLabel, statusWidth);
                        });

                        packageScrollPosition = EditorGUILayout.BeginScrollView(packageScrollPosition);
                        foreach (var package in packages)
                        {
                            this.HGroup(() =>
                            {
                                try
                                {
                                    EditorGUILayout.LabelField(package.GUILabel, nameFieldGWidth);
                                    package.CompilerDefine = EditorGUILayout.TextField(package.CompilerDefine, nameFieldGWidth);
                                    EditorGUILayout.LabelField(
                                        DesiredConfiguration.CompilerDefines.Contains(package.CompilerDefine) ? "Yes" : "No",
                                        EditorStyles.centeredGreyMiniLabel,
                                        narrowGWidth);

                                    if (package.ScanningProgress == PackageScanStatus.None)
                                    {
                                        EditorGUILayout.LabelField("Identified", EditorStyles.centeredGreyMiniLabel, statusWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.Found
                                        || package.ScanningProgress == PackageScanStatus.List)
                                    {
                                        EditorGUILayout.LabelField("Found", EditorStyles.centeredGreyMiniLabel, statusWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.NotFound)
                                    {
                                        EditorGUILayout.LabelField("Not Found!", statusWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.Listing)
                                    {
                                        EditorGUILayout.LabelField("Listing", EditorStyles.centeredGreyMiniLabel, statusWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.Listed
                                        || package.ScanningProgress == PackageScanStatus.Scan
                                        || package.ScanningProgress == PackageScanStatus.Scanning)
                                    {
                                        EditorGUILayout.LabelField(string.Format(
                                            "({0} files) Scanning",
                                            package.TotalFiles),
                                            EditorStyles.centeredGreyMiniLabel,
                                            statusWidth);
                                    }
                                    else
                                    {
                                        if (package.ScanningProgress == PackageScanStatus.Scanned)
                                        {
                                            EditorGUILayout.LabelField(string.Format(
                                                "({0} of {1} files)",
                                                Units.Converter.Label(package.InstallPercentage, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent),
                                                package.TotalFiles),
                                                EditorStyles.centeredGreyMiniLabel,
                                                statusWidth);

                                            var installLabel = package.InstallPercentage == 0 ? "Install" : "Refresh";
                                            if (package.InstallPercentage == 1)
                                            {
                                                GUILayout.Space(buttonWidth);
                                            }
                                            else if (GUILayout.Button(installLabel, buttonGWidth))
                                            {
                                                using (var prog = new UnityEditorProgressDialog("Installing " + package.Name))
                                                {
                                                    package.Install(prog);
                                                }
                                            }

                                            if (package.InstallPercentage > 0 && GUILayout.Button("Uninstall", buttonGWidth))
                                            {
                                                using (var prog = new UnityEditorProgressDialog("Uninstalling " + package.Name))
                                                {
                                                    package.Uninstall(prog);
                                                }
                                            }
                                        }
                                        else if (package.ScanningProgress == PackageScanStatus.Error)
                                        {
                                            if (GUILayout.Button(
                                                new GUIContent("ERROR!", package.Error.Message),
                                                EditorStyles.miniBoldLabel,
                                                buttonGWidth))
                                            {
                                                package.ClearError();
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            });
                        }
                        EditorGUILayout.EndScrollView();

                        platforms.Save();
                    }
                });

                //this.HeaderIndent("Defines", () =>
                //{
                //    var defines = CleanupDefines(PlayerSettings.GetScriptingDefineSymbolsForGroup(CurrentConfiguration.TargetGroup)
                //        .SplitX(';'));
                //    var nextDefines = defines.ToList();

                //    this.HGroup(() =>
                //    {
                //        EditorGUILayout.LabelField("Define", EditorStyles.centeredGreyMiniLabel, nameFieldGWidth);
                //        EditorGUILayout.LabelField("Required", EditorStyles.centeredGreyMiniLabel, buttonGWidth);
                //    });

                //    this.HGroup(() =>
                //    {
                //        newDefine = EditorGUILayout.TextField(newDefine, GUILayout.Width(nameFieldWidth + narrowWidth));
                //        if (GUILayout.Button("Add", buttonGWidth))
                //        {
                //            if (!string.IsNullOrEmpty(newDefine))
                //            {
                //                nextDefines.Add(newDefine);
                //            }
                //            newDefine = string.Empty;
                //        }
                //    });


                //    definesScrollPosition = EditorGUILayout.BeginScrollView(definesScrollPosition);
                //    for (var i = 0; i < nextDefines.Count; ++i)
                //    {
                //        this.HGroup(() =>
                //        {
                //            var define = nextDefines[i];
                //            EditorGUILayout.LabelField(new GUIContent(define, define), nameFieldGWidth);

                //            EditorGUILayout.LabelField(
                //                DesiredConfiguration.CompilerDefines.Contains(define) ? "Yes" : "No",
                //                EditorStyles.centeredGreyMiniLabel,
                //                narrowGWidth);

                //            if (GUILayout.Button("Remove", buttonGWidth))
                //            {
                //                nextDefines.RemoveAt(i);
                //                --i;
                //            }
                //        });
                //    }
                //    EditorGUILayout.EndScrollView();

                //    nextDefines = CleanupDefines(nextDefines);

                //    if (!nextDefines.Matches(defines))
                //    {
                //        PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentConfiguration.TargetGroup, string.Join(";", nextDefines));
                //    }
                //});
            }
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

        private static PlatformTypes DesiredPlatform
        {
            get
            {
                return config.CurrentPlatform;
            }
        }

        private static PlatformConfiguration DesiredConfiguration
        {
            get
            {
                return platforms.PlatformDB.Get(DesiredPlatform);
            }
        }

        private static PlatformTypes CurrentPlatform
        {
            get
            {
                return JuniperPlatform.CurrentPlatform;
            }
        }

        private static PlatformConfiguration CurrentConfiguration
        {
            get
            {
                return platforms.PlatformDB.Get(CurrentPlatform);
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

        private static PlatformConfiguration NextConfiguration
        {
            get
            {
                return platforms.PlatformDB.Get(NextPlatform);
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

        private static void OnEditorUpdate(Action resolve, Action reject, Func<bool> test)
        {
            if (test == null)
            {
                test = AlwaysTrue;
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

        private static bool AlwaysTrue() { return true; }

        private static void OnEditorUpdate(Action resolve, Action reject)
        {
            OnEditorUpdate(resolve, reject, null);
        }

        private static void OnEditorUpdate(Action resolve, Func<bool> test)
        {
            OnEditorUpdate(resolve, null, test);
        }

        private static void OnEditorUpdate(Action resolve)
        {
            OnEditorUpdate(resolve, null, null);
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

        private static IProgress PrepareBuildStep(int offset, string prefix)
        {
            LastPrefix = prefix ?? LastPrefix;
            var step = BuildProgress - offset + 1;
            var msg = $"Juniper ({step} of {STAGES.Length}): {LastPrefix}";
            return new UnityEditorProgressDialog(msg)
                .Subdivide(step + BuildProgress, 2 * STAGES.Length, LastPrefix);
        }

        private static IProgress PrepareBuildStep(int offset)
        {
            return PrepareBuildStep(offset, null);
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
                CurrentConfiguration.Deactivate(NextConfiguration);
                Recompile(true, CurrentConfiguration, NextConfiguration);
            });
        }

        public static void RefreshPackages()
        {
            WithProgress("Refreshing packages " + NextPlatform, prog =>
            {
                var progs = prog.Split(6);
                CurrentConfiguration.UninstallAssetStorePackages(progs[0]);
                CurrentConfiguration.UninstallZipPackages(progs[1]);
                var manifest = UpdateUnityPackages();
                CurrentConfiguration.UninstallUnityPackages(progs[2]);
                NextConfiguration.InstallUnityPackages(progs[3]);

                var txt = manifest.ToString(Formatting.Indented);
                FileExt.WriteAllText(MANIFEST_FILE, txt);

                NextConfiguration.InstallZipPackages(progs[4]);
                NextConfiguration.InstallAssetStorePackages(progs[5]);

                InternalCompile(!NextConfiguration.TargetSwitchNeeded, NextConfiguration);
                if (NextConfiguration.TargetSwitchNeeded)
                {
                    NextConfiguration.SwitchTarget();
                }
                else
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
                    if (!EditorApplication.isCompiling)
                    {
                        ResumeBuild();
                    }
                }
            });
        }

        private static JObject UpdateUnityPackages()
        {
            var manifest = JObject.Parse(File.ReadAllText(MANIFEST_FILE));
            var deps = (JObject)manifest["dependencies"];
            if (deps == null)
            {
                deps = new JObject();
                manifest.Add("dependencies", deps);
            }

            UnityPackage.Dependencies = deps;
            return manifest;
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
