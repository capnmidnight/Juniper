using System;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Juniper.CustomEditors
{
    public abstract class StreamableAssetEditor<T, U> : PropertyDrawer,
        IPreprocessBuildWithReport, IPostprocessBuildWithReport
        where T : UnityEngine.Object
        where U : StreamableAsset<T>, new()
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            if (property.isArray)
            {
                EditorGUI.PropertyField(position, property, false);
            }
            else
            {
                var obj = (U)fieldInfo.GetValue(property.serializedObject.targetObject);
                obj.Validate();
                var assetProp = property.FindPropertyRelative(nameof(obj.Asset));
                EditorGUI.ObjectField(position, assetProp, typeof(T), label);
            }

            EditorGUI.EndProperty();
        }

        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("Juniper: Exporting streaming assets.");
            ForEachStreamable(value =>
            {
                Debug.Log("Juniper: Exporting " + value.AssetPath + " to " + value.CopyPath);
                value.Export();
            });
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            ForEachStreamable(value => value.Import());
        }

        private void ForEachStreamable(Action<StreamableAsset> act)
        {
            for (var i = 0; i < SceneManager.sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var obj in root.GetComponentsInChildren<Component>(true))
                    {
                        var t = obj.GetType();
                        foreach (var field in t.GetFields())
                        {
                            var val = field.GetValue(obj);
                            if (val is StreamableAsset)
                            {
                                var value = (StreamableAsset)val;
                                if (!string.IsNullOrEmpty(value.AssetPath))
                                {
                                    act(value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
