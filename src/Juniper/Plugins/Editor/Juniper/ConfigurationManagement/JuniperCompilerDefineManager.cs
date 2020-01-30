using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
    public class JuniperCompilerDefineManager : EditorWindow
    {
        [MenuItem(MENU_NAME + "Compiler Defines Manager")]
        public static void ShowJuniperWindow()
        {
            EditorWindow.GetWindow<JuniperCompilerDefineManager>();
        }

        private static readonly GUIContent TITLE = new GUIContent("Juniper - Compiler Define Symbols");
        private const string MENU_NAME = "Juniper/";

        private static string newDefine;

        private const float narrowWidthValue = 50;

        private static readonly GUILayoutOption narrowWidth = GUILayout.Width(narrowWidthValue);
        private static readonly GUILayoutOption buttonWidth = GUILayout.Width(100);

        private static readonly TableView definesTable = new TableView(
            "Defines - " + ProjectConfiguration.Platform,
            ("Define", null),
            ("Required", buttonWidth)
        );


        public void OnGUI()
        {
            titleContent = TITLE;

            var platforms = Platforms.Load();
            var selectedPlatform = ProjectConfiguration.Platform;
            var currentConfiguration = platforms.Configurations[selectedPlatform];
            var requiredDefines = currentConfiguration.GetCompilerDefines();
            var curDefines = Project.GetDefines();

            var allDefines = curDefines
                .Union(requiredDefines)
                .Distinct()
                .OrderBy(Always.Identity)
                .ToArray();

            using (definesTable.Begin())
            {
                using (new HGroup())
                {
                    newDefine = EditorGUILayout.TextField(newDefine);
                    EditorGUILayout.LabelField(string.Empty, narrowWidth);
                    if (GUILayout.Button("Add", buttonWidth))
                    {
                        if (!string.IsNullOrEmpty(newDefine))
                        {
                            Project.AddCompilerDefine(newDefine);
                        }
                        newDefine = string.Empty;
                    }
                }

                foreach (var define in allDefines)
                {
                    using (new HGroup())
                    {
                        EditorGUILayout.LabelField(new GUIContent(define, define));

                        EditorGUILayout.LabelField(
                            requiredDefines.Contains(define).ToYesNo(),
                            EditorStyles.centeredGreyMiniLabel,
                            narrowWidth);

                        var isCurrent = curDefines.Contains(define);

                        using (new EnabledScope(!isCurrent))
                        {
                            if (GUILayout.Button("Add", buttonWidth))
                            {
                                Project.AddCompilerDefine(define);
                            }
                        }

                        using (new EnabledScope(isCurrent))
                        {
                            if (GUILayout.Button("Remove", buttonWidth))
                            {
                                Project.RemoveCompilerDefine(define);
                            }
                        }
                    }
                }
            }
        }
    }
}
