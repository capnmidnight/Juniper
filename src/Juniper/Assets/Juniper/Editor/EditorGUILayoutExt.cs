using UnityEngine;

namespace UnityEditor
{
    public static class EditorGUILayoutExt
    {
        public static void ShowScriptField(MonoBehaviour value)
        {
            var script = MonoScript.FromMonoBehaviour(value);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }

        public static void LabeledField<T>(string label, GUILayoutOption labelWidth, T value, GUILayoutOption valueWidth)
        {
            using (_ = new HGroup())
            {
                EditorGUILayout.LabelField(label, labelWidth);
                EditorGUILayout.LabelField(value.ToString(), valueWidth);
            }
        }

        public static void LabeledField<T>(string label, GUILayoutOption labelWidth, T value)
        {
            using (_ = new HGroup())
            {
                EditorGUILayout.LabelField(label, labelWidth);
                EditorGUILayout.LabelField(value.ToString());
            }
        }
    }
}
