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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= indent;
            }
        }
    }
}
