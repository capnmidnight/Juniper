using System.Collections.Generic;
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
    public class JuniperInstalledPackageManager : EditorWindow
    {
        [MenuItem(MENU_NAME + "Installed Packages")]
        public static void ShowJuniperWindow()
        {
            EditorWindow.GetWindow<JuniperInstalledPackageManager>();
        }

        private const string MENU_NAME = "Juniper/";
        private static readonly GUIContent TITLE = new GUIContent("Juniper - Installed Packages");

        private const int buttonWidthValue = 100;

        private static readonly GUILayoutOption nameFieldWidth = GUILayout.Width(200);
        private static readonly GUILayoutOption buttonWidth = GUILayout.Width(buttonWidthValue);

        private static readonly TableView installedPackageTable = new TableView(
            "Installed Packages",
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

                var defines = Project.GetDefines();

                DrawInstalledPackages(defines);
            }
        }

        private static void DrawInstalledPackages(List<string> defines)
        {
            var installed = from versions in Project.PackageDatabase.Values
                            from package in versions
                            where package.IsInstalled
                            orderby package.Source, package.PackageID, package.VersionSpec
                            select package;

            using (installedPackageTable.Begin())
            {
                var drawn = 0;
                foreach (var pkg in installed)
                {
                    ++drawn;
                    using (new HGroup())
                    {
                        EditorGUILayout.LabelField(pkg.Source.ToString(), nameFieldWidth);
                        EditorGUILayout.LabelField(pkg.Name);
                        EditorGUILayout.LabelField(pkg.VersionSpec, nameFieldWidth);

                        using (new EnabledScope(!defines.Contains(pkg.CompilerDefine)))
                        {
                            if (GUILayout.Button("Activate", buttonWidth))
                            {
                                pkg.Activate();
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
