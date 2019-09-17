using System;
using UnityEngine;

namespace UnityEditor
{
    public static class EditorWindowExt
    {
        public static void HeaderIndent(this EditorWindow window, string label, Action content)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
            content();
            --EditorGUI.indentLevel;
        }

        public static void HGroup(this EditorWindow window, Action content)
        {
            EditorGUILayout.BeginHorizontal();
            content();
            EditorGUILayout.EndHorizontal();
        }
    }
}
