using System;

namespace UnityEditor
{

    public class Header : IDisposable
    {
        public Header(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            ++EditorGUI.indentLevel;
        }

        public void Dispose()
        {
            --EditorGUI.indentLevel;
        }
    }
}
