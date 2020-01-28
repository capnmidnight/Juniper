using System;

using UnityEditor;

namespace Juniper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class EnabledScope :
        IDisposable
    {
        public  EnabledScope(bool enabled)
        {
            EditorGUI.BeginDisabledGroup(!enabled);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "<Pending>")]
        public void Dispose()
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}
