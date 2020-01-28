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
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

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
                var curPoint = property.GetObject<LatLngPoint>();
                var curLat = curPoint.Latitude.ToString();
                var curLng = curPoint.Longitude.ToString();
                EditorGUIUtility.labelWidth = 25;
                var nextLat = EditorGUI.TextField(latRect, labelLat, curLat);
                var nextLng = EditorGUI.TextField(lngRect, labelLng, curLng);
                if (float.TryParse(nextLat, out var lat)
                    && float.TryParse(nextLng, out var lng))
                {
                    curPoint = new LatLngPoint(lat, lng);
                    property.SetValue(curPoint);
                }
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