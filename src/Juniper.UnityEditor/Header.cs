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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                --EditorGUI.indentLevel;
            }
        }
    }
}
