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
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            var value = (EditorTextInput)serializedObject.targetObject;
            EditorGUILayoutExt.ShowScriptField(value);

            if (GUILayout.Button("Submit"))
            {
                value.Submit();
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}
