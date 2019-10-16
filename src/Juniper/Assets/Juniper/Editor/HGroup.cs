using System;

namespace UnityEditor
{
    public class HGroup : IDisposable
    {
        private readonly int indent;
        public HGroup(int indent = 0)
        {
            this.indent = indent;
            EditorGUI.indentLevel += indent;
            EditorGUILayout.BeginHorizontal();
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel -= indent;
        }
    }
}
