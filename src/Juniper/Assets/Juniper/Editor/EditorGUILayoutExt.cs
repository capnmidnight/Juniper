using System;
using UnityEngine;

namespace UnityEditor
{
    public static class EditorGUILayoutExt
    {
        public static void HeaderIndent(string label, Action content)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            content();
            --EditorGUI.indentLevel;
        }

        public static void HGroup(Action content)
        {
            EditorGUILayout.BeginHorizontal();
            content();
            EditorGUILayout.EndHorizontal();
        }

        public static void ShowScriptField(MonoBehaviour value)
        {
            var script = MonoScript.FromMonoBehaviour(value);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }
    }
}
