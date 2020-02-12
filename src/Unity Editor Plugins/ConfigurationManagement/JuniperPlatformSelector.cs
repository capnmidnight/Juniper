using System.Diagnostics.CodeAnalysis;

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
    public class JuniperPlatformSelector : EditorWindow
    {
        [MenuItem(MENU_NAME + "Select Platform")]
        public static void ShowJuniperWindow()
        {
            EditorWindow.GetWindow<JuniperPlatformSelector>();
        }

        private const string MENU_NAME = "Juniper/";
        private static readonly GUIContent TITLE = new GUIContent("Juniper - Platform");

        private const int buttonWidthValue = 100;

        private static readonly GUILayoutOption nameFieldWidth = GUILayout.Width(200);
        private static readonly GUILayoutOption buttonWidth = GUILayout.Width(buttonWidthValue);

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

                DrawStatus(platforms, selectedPlatform, currentConfiguration, targetGroup);
            }
        }

        private static void DrawStatus(Platforms platforms, PlatformType selectedPlatform, PlatformConfiguration currentConfiguration, BuildTargetGroup targetGroup)
        {
            using (new Header("Status"))
            {
                EditorGUILayoutExt.LabeledField("Current platform", nameFieldWidth, selectedPlatform, nameFieldWidth);
                EditorGUILayoutExt.LabeledField("Current build target", nameFieldWidth, targetGroup, nameFieldWidth);

                if (targetGroup == BuildTargetGroup.Android)
                {
                    EditorGUILayoutExt.LabeledField("Min Android SDK version", nameFieldWidth, currentConfiguration.AndroidSdkVersion, nameFieldWidth);
                }

                using (new HGroup())
                {
                    EditorGUILayout.LabelField("Change platform", nameFieldWidth);
                    selectedPlatform = (PlatformType)EditorGUILayout.EnumPopup(selectedPlatform, nameFieldWidth);
                    if (selectedPlatform != ProjectConfiguration.Platform)
                    {
                        var nextPlatform = platforms.Configurations[selectedPlatform];
                        if (nextPlatform.IsSupported())
                        {
                            ProjectConfiguration.Platform = nextPlatform.Name;
                            nextPlatform.SwitchTarget();
                        }
                        else
                        {
                            EditorUtility.DisplayDialog(
                               "Juniper",
                               $"The platform {selectedPlatform} is not supported on your current system.",
                               "OK");
                        }
                    }
                }

                using (new EnabledScope(!currentConfiguration.IsActivated()))
                {
                    if (GUILayout.Button("Activate", buttonWidth))
                    {
                        currentConfiguration.Activate();
                    }
                }
            }
        }
    }
}
