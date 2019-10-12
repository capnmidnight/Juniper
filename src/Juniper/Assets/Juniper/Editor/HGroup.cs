using System;

namespace UnityEditor
{
    public class HGroup : IDisposable
    {
        public HGroup()
        {
            EditorGUILayout.BeginHorizontal();
        }

        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}
