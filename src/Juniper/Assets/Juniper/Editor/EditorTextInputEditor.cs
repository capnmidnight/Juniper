using UnityEditor;

using UnityEngine;

namespace Juniper.Unity.Editor
{
    [CustomEditor(typeof(EditorTextInput))]
    public class EditorTextInputEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Submit"))
            {
                var editor = (EditorTextInput)serializedObject.targetObject;
                editor.Submit();
            }
        }
    }
}
