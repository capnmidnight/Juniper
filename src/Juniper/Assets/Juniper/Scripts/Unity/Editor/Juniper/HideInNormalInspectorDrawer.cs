using Juniper.Unity;

using UnityEditor;

using UnityEngine;

namespace Juniper.UnityEditor
{
    [CustomPropertyDrawer(typeof(HideInNormalInspectorAttribute))]
    public class HideInNormalInspectorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}
