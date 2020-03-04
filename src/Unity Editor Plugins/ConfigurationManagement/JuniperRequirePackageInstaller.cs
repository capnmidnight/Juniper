using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Juniper.XR;

using UnityEditor;

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
    public class JuniperRequiredPackageInstaller : EditorWindow
    {
        [MenuItem(MENU_NAME + "Required Packages")]
        public static void ShowJuniperWindow()
        {
            EditorWindow.GetWindow<JuniperRequiredPackageInstaller>();
        }

        private const string MENU_NAME = "Juniper/";
        private static readonly GUIContent TITLE = new GUIContent("Juniper");

        private const int buttonWidthValue = 100;

        private static readonly GUILayoutOption nameFieldWidth = GUILayout.Width(200);
        private static readonly GUILayoutOption buttonWidth = GUILayout.Width(buttonWidthValue);

        private static readonly TableView requiredPackageTable = new TableView(
            "Required Packages",
            ("Source", nameFieldWidth),
            ("Name", null),
            ("Version", nameFieldWidth),
            ("", buttonWidth)
        );

        public void OnGUI()
        {
            titleContent = TITLE;

            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Compiling...");
            }
            else
            {
                var platforms = Platforms.Load();
                var selectedPlatform = ProjectConfiguration.Platform;
                var currentConfiguration = platforms.Configurations[selectedPlatform];
                var targetGroup = currentConfiguration.GetTargetGroup();

                DrawStatus(selectedPlatform, targetGroup);

                var manifest = UnityPackageManifest.Load();

                var requiredPackages = platforms.Packages
                    .Union(currentConfiguration.Packages)
                    .ToArray();

                DrawRequiredPackages(manifest, requiredPackages);
            }
        }

        private static void DrawRequiredPackages(UnityPackageManifest manifest, PackageReference[] requiredPackages)
        {
            var needsInstall = from req in requiredPackages
                               where (req.ForRemoval
                                    && manifest.ContainsKey(req.PackageID))
                                || (!req.ForRemoval
                                    && !Project.IsInstalled(req))
                               orderby req.Source, req.PackageID, req.VersionSpec
                               select req;

            using (requiredPackageTable.Begin())
            {
                var drawn = 0;
                foreach (var req in needsInstall)
                {
                    ++drawn;
                    using (new HGroup())
                    {
                        var isAvailable = Project.IsAvailable(req) || req.ForRemoval;

                        EditorGUILayout.LabelField(req.Source.ToString(), nameFieldWidth);
                        EditorGUILayout.LabelField(req.Name);
                        EditorGUILayout.LabelField(req.VersionSpec, nameFieldWidth);

                        using (new EnabledScope(isAvailable))
                        {
                            if (GUILayout.Button(isAvailable
                                ? req.ForRemoval
                                    ? "Remove"
                                    : "Install"
                                : "Not available", buttonWidth))
                            {
                                if (req.ForRemoval)
                                {
                                    manifest.Remove(req);
                                }
                                else
                                {
                                    var pkg = Project.PackageDatabase.ContainsKey(req.PackageID)
                                        ? Project.PackageDatabase[req.PackageID].FirstOrDefault(req.Equals)
                                        : null;
                                    if (pkg is null)
                                    {
                                        Debug.LogWarning("Couldn't find pakage " + req);
                                    }
                                    else
                                    {
                                        pkg.Install();
                                    }
                                }
                            }
                        }
                    }
                }

                if (drawn == 0)
                {
                    EditorGUILayout.LabelField("None", EditorStyles.centeredGreyMiniLabel);
                }
            }
        }

        private static void DrawStatus(PlatformType selectedPlatform, BuildTargetGroup targetGroup)
        {
            using (new Header("Status"))
            {
                EditorGUILayoutExt.LabeledField("Current platform", nameFieldWidth, selectedPlatform, nameFieldWidth);
                EditorGUILayoutExt.LabeledField("Current build target", nameFieldWidth, targetGroup, nameFieldWidth);
            }
        }
    }
}