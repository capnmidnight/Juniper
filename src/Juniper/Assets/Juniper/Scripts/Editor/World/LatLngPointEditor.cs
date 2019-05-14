using UnityEditor;

using UnityEngine;

namespace Juniper.World.GIS
{
    /// <summary>
    /// An editor for <see cref="LatLngPoint"/> s.
    /// </summary>
    [CustomPropertyDrawer(typeof(LatLngPoint))]
    public class LatLngPointEditor : PropertyDrawer
    {
        /// <summary>
        /// Builds the GUI for editing <see cref="LatLngPoint"/> s.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (property.isArray)
            {
                EditorGUI.PropertyField(position, property, false);
            }
            else
            {
                var latRect = new Rect(position.x, position.y, position.width / 2, position.height);
                var lngRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
                var latProp = property.FindPropertyRelative("Latitude");
                var lngProp = property.FindPropertyRelative("Longitude");
                EditorGUIUtility.labelWidth = 25;
                EditorGUI.PropertyField(latRect, latProp, labelLat);
                EditorGUI.PropertyField(lngRect, lngProp, labelLng);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// A GUI label for the points Latitude
        /// </summary>
        private static readonly GUIContent labelLat = new GUIContent("Lat", "Latitude");

        /// <summary>
        /// A GUI label for the points Longitude
        /// </summary>
        private static readonly GUIContent labelLng = new GUIContent("Lng", "Longitude");
    }
}
