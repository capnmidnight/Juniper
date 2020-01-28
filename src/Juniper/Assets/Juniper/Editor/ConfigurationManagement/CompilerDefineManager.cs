using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
    public class CompilerDefineManager : EditorWindow
    {
        private static readonly GUIContent TITLE = new GUIContent("Compiler Defines");
        private const string MENU_NAME = "Juniper/";

        [MenuItem(MENU_NAME + "Compiler Defines Manager")]
        public static void ShowJuniperWindow()
        {
            EditorWindow.GetWindow<CompilerDefineManager>();
        }

        private static string newDefine;

        private const float nameFieldWidthValue = 200;
        private const float narrowWidthValue = 50;

        private static readonly GUILayoutOption nameFieldWidth = GUILayout.Width(nameFieldWidthValue);
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

            var curDefines = Project.GetDefines();

            var platformDefines = PlatformConfiguration.Current.GetCompilerDefines();
             
            var allDefines = Platforms.Load()
                .Configurations.Values
                .Select(cfg => cfg.CompilerDefine)
                .Union(curDefines)
                .Union(platformDefines)
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
                            platformDefines.Contains(define).ToYesNo(),
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
