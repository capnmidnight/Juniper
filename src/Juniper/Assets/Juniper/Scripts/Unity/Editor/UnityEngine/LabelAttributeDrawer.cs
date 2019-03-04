using UnityEditor;

namespace UnityEngine
{
    /// <summary>
    /// A custom property drawer for drawing customized labels on Unity component fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// Render the <paramref name="label"/>.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyAttribute = attribute as LabelAttribute;
            label.text = propertyAttribute.label;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
