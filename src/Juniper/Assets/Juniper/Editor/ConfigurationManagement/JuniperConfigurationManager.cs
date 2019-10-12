using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

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
        private static readonly GUIContent TITLE = new GUIContent("Juniper");

        private static readonly ProjectConfiguration config = ProjectConfiguration.Load();

        private static AbstractFilePackage[] packages;
        private static bool repaintNeeded;
        private static bool repaintBound;
        private static readonly ProgressEventer currentProg = new ProgressEventer();

        private static string progressMessage;

        static JuniperConfigurationManager()
        {
            Platforms.PackagesUpdated += Platforms_PackagesUpdated;
            Platforms.ScanningProgressUpdated += RepaintWindow;

            config.PlatformChanged += Config_PlatformChanged;
            config.PlatformChangeConfirmed += StartBuild;

            currentProg.ProgressUpdated += CurrentProg_ProgressUpdated;

            if (RebuildNeeded)
            {
                OnEditorUpdate(() =>
                    NextPlatform = DesiredPlatform,
                    () => EditorUtility.DisplayDialog("Juniper", "Could not change platform.", "OK"));
            }
        }

        private static void CurrentProg_ProgressUpdated(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            var pips = Mathf.RoundToInt(currentProg.Progress * 10);
            var negPips = 10 - pips;
            for (var i = 0; i < pips; ++i)
            {
                sb.Append('=');
            }

            for (var i = 0; i < negPips; ++i)
            {
                sb.Append('-');
            }

            sb.Append("] (");
            sb.Append(Units.Converter.Label(currentProg.Progress, Units.UnitOfMeasure.Proportion, Units.UnitOfMeasure.Percent, 2));
            sb.Append(") :> ");
            sb.Append(currentProg.Status);
            var message = sb.ToString();
            if (message != progressMessage)
            {
                progressMessage = message;
                RepaintWindow();
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

            if (!Platforms.IsRunning)
            {
                Platforms.StartFileWatcher();
            }
        }

        private const float nameFieldWidth = 200;
        private const float narrowWidth = 50;
        private const float statusWidth = 120;
        private const float buttonWidth = 100;

        private static readonly GUILayoutOption nameFieldGWidth = GUILayout.Width(nameFieldWidth);
        private static readonly GUILayoutOption narrowGWidth = GUILayout.Width(narrowWidth);
        private static readonly GUILayoutOption statusGWidth = GUILayout.Width(statusWidth);
        private static readonly GUILayoutOption buttonGWidth = GUILayout.Width(buttonWidth);

        private static readonly TableView requiredPackageTable = new TableView(
            ("Name", nameFieldWidth),
            ("Define", nameFieldWidth),
            ("Active", narrowWidth),
            ("Status", statusWidth)
        );

        private static readonly TableView optionalPackageTable = new TableView(
            ("Name", nameFieldWidth),
            ("Define", nameFieldWidth),
            ("Active", narrowWidth),
            ("Status", statusWidth)
        );


        public void OnGUI()
        {
            titleContent = TITLE;

            if (!repaintBound)
            {
                repaintBound = true;
                EditorApplication.update += EditorUpdate;

            }

            repaintNeeded = false;

            var selectedPlatform = CurrentPlatform;

            using (_ = new Header("Status"))
            {
                if (BuildInProgress)
                {
                    EditorGUILayoutExt.LabeledField("Build in progress", nameFieldGWidth, progressMessage);
                }

                EditorGUILayoutExt.LabeledField("Build step", nameFieldGWidth, BuildStepName, nameFieldGWidth);

                if (!BuildInProgress)
                {
                    EditorGUILayoutExt.LabeledField("Current platform", nameFieldGWidth, CurrentPlatform, nameFieldGWidth);
                    EditorGUILayoutExt.LabeledField("Current build target", nameFieldGWidth, CurrentConfiguration.TargetGroup, nameFieldGWidth);

                    if (CurrentConfiguration.TargetGroup == BuildTargetGroup.Android)
                    {
                        EditorGUILayoutExt.LabeledField("Min Android SDK version", nameFieldGWidth, CurrentConfiguration.androidSdkVersion, nameFieldGWidth);
                    }

                    using (_ = new HGroup())
                    {
                        EditorGUILayout.LabelField("Change platform", nameFieldGWidth);
                        selectedPlatform = (PlatformTypes)EditorGUILayout.EnumPopup(selectedPlatform, nameFieldGWidth);
                    }
                }
            }

            if (!BuildInProgress)
            {
                if (selectedPlatform != CurrentPlatform)
                {
                    if (Platforms.PlatformDB[selectedPlatform].IsSupported)
                    {
                        NextPlatform = selectedPlatform;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                           "Juniper",
                           $"The platform {selectedPlatform} is not supported on your current system.",
                           "OK");
                    }
                }
                else
                {
                    UpdateUnityPackages();
                    var nextDefines = UnityCompiler.GetDefines(CurrentConfiguration.TargetGroup);
                    var packages = JuniperConfigurationManager.packages;

                    if (packages == null)
                    {
                        EditorGUILayout.LabelField("(Loading)", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        DrawPackages("Required", packages.Where(p => nextDefines.Contains(p.CompilerDefine)), nextDefines, requiredPackageTable);
                        DrawPackages("Optional", packages.Where(p => !nextDefines.Contains(p.CompilerDefine)), nextDefines, optionalPackageTable);
                    }

                    UnityCompiler.SetDefines(CurrentConfiguration.TargetGroup, nextDefines);
                }
            }
        }

        private void DrawPackages(string label, IEnumerable<AbstractFilePackage> packages, List<string> nextDefines, TableView table)
        {
            using (_ = new Header(label + " Packages"))
            {
                if (packages.Empty())
                {
                    EditorGUILayout.LabelField("(Loading)", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    using (_ = table.Begin())
                    {
                        foreach (var package in packages.OrderBy(p => p.CompilerDefine))
                        {
                            using (_ = new HGroup())
                            {
                                try
                                {
                                    EditorGUILayout.LabelField(package.GUILabel, nameFieldGWidth);
                                    package.CompilerDefine = EditorGUILayout.TextField(package.CompilerDefine, nameFieldGWidth);

                                    EditorGUILayout.LabelField(
                                        CurrentConfiguration.CompilerDefines.Contains(package.CompilerDefine) ? "Yes" : "No",
                                        EditorStyles.centeredGreyMiniLabel,
                                        narrowGWidth);

                                    if (package.ScanningProgress == PackageScanStatus.None)
                                    {
                                        EditorGUILayout.LabelField("Identified", EditorStyles.centeredGreyMiniLabel, statusGWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.Found
                                        || package.ScanningProgress == PackageScanStatus.List)
                                    {
                                        EditorGUILayout.LabelField("Found", EditorStyles.centeredGreyMiniLabel, statusGWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.NotFound)
                                    {
                                        EditorGUILayout.LabelField("Not Found!", statusGWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.Listing
                                        || package.ScanningProgress == PackageScanStatus.Listed
                                        || package.ScanningProgress == PackageScanStatus.Scan
                                        || package.ScanningProgress == PackageScanStatus.Scanning)
                                    {
                                        EditorGUILayout.LabelField(string.Format(
                                            "({0} files) Scanning",
                                            package.TotalFiles),
                                            EditorStyles.centeredGreyMiniLabel,
                                            statusGWidth);
                                    }
                                    else if (package.ScanningProgress == PackageScanStatus.Scanned)
                                    {
                                        if (package.InstallPercentage > 0 != nextDefines.Contains(package.CompilerDefine))
                                        {
                                            if (package.InstallPercentage > 0)
                                            {
                                                nextDefines.MaybeAdd(package.CompilerDefine);
                                            }
                                            else
                                            {
                                                nextDefines.Remove(package.CompilerDefine);
                                            }
                                        }

                                        EditorGUILayout.LabelField(string.Format(
                                            "({0,4:##0%} of {1,4:####} files)",
                                            package.InstallPercentage,
                                            package.TotalFiles),
                                            EditorStyles.centeredGreyMiniLabel,
                                            statusGWidth);

                                        var installLabel = package.InstallPercentage == 0 ? "Install" : "Refresh";
                                        if (package.InstallPercentage == 1)
                                        {
                                            GUILayout.Space(buttonWidth);
                                        }
                                        else if (GUILayout.Button(installLabel, buttonGWidth))
                                        {
                                            package.Install(currentProg);
                                        }

                                        if (package.InstallPercentage > 0 && GUILayout.Button("Remove", buttonGWidth))
                                        {
                                            package.Uninstall(currentProg);
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
                                catch
                                {

                                }
                            }
                        }
                    }

                    Platforms.Save();
                }
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
                return Platforms.PlatformDB.Get(DesiredPlatform);
            }
        }

        private static PlatformTypes CurrentPlatform
        {
            get
            {
                return JuniperSystem.CurrentPlatform;
            }
        }

        private static PlatformConfiguration CurrentConfiguration
        {
            get
            {
                return Platforms.PlatformDB.Get(CurrentPlatform);
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
                return Platforms.PlatformDB.Get(NextPlatform);
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
                return DesiredPlatform != PlatformTypes.None
                    && DesiredPlatform != CurrentPlatform
                    && !BuildInProgress;
            }
        }

        private static bool Config_PlatformChanged()
        {
            if (!Platforms.PlatformDB[config.NextPlatform].IsSupported)
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
                var d = Platforms.AllCompilerDefines.ToList();
                d.Add(RECOMPILE_SLUG);
                return d.Distinct().ToArray();
            }
        }

        #region Menu

        private static bool MenuCheck(PlatformTypes p)
        {
            return CurrentPlatform != p && Platforms.PlatformDB[p].IsSupported;
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
                test = Always.True;
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

        private static void OnEditorUpdate(Action resolve, Func<bool> test)
        {
            OnEditorUpdate(resolve, null, test);
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
            return currentProg
                .Subdivide(step + BuildProgress, 2 * STAGES.Length, LastPrefix + msg);
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

            var targetGroups = platforms
                .Select(p => p.TargetGroup)
                .Distinct()
                .Reverse()
                .ToArray();

            foreach (var targetGroup in targetGroups)
            {
                UnityCompiler.SetDefines(targetGroup, nextDefines, true);
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

        private static readonly string MANIFEST_FILE = PathExt.FixPath("Packages/manifest.json");

        private static void StartBuild()
        {
            BuildProgress = 0;
            DeactivatePlatform();
        }

        private static void DeactivatePlatform()
        {
            WithProgress("Resetting to base configuration", _ =>
            {
                JuniperSystem.UninstallSystem();
                CurrentConfiguration.Deactivate();
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
                JuniperSystem.InstallSystem(true);
                config.Commit();
                OnCancel(false);
            });
        }
    }
}
