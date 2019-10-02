using UnityEngine;

namespace UnityEditor
{
    public static class EditorExt
    {
        public static void ShowScriptField(this Editor editor, MonoBehaviour value)
        {
            var script = MonoScript.FromMonoBehaviour(value);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }
    }
}
