using Juniper.Unity;
using System;

using UnityEditor;
using UnityEditor.Build;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Juniper.UnityEditor
{
    public abstract class StreamableAssetEditor<T, U> : PropertyDrawer,
#if UNITY_2018_1_OR_NEWER
        IPreprocessBuildWithReport, IPostprocessBuildWithReport
#else
        IPreprocessBuild, IPostprocessBuild
#endif
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

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif
        {
            Debug.Log("Juniper: Exporting streaming assets.");
            ForEachStreamable(value =>
            {
                Debug.Log("Juniper: Exporting " + value.AssetPath + " to " + value.CopyPath);
                value.Export();
            });
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(BuildReport report)
#else
        public void OnPostprocessBuild(BuildTarget target, string path)
#endif
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
