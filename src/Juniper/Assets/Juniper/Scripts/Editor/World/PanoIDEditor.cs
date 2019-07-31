using System;
using System.Reflection;
using Juniper.Google.Maps.StreetView;
using UnityEditor;

using UnityEngine;

namespace Juniper.Unity.Editor
{
    /// <summary>
    /// An editor for <see cref="PanoID"/> s.
    /// </summary>
    [CustomPropertyDrawer(typeof(PanoID))]
    public class PanoIDEditor : PropertyDrawer
    {
        private static readonly Type[] paramTypes = new[] { typeof(string) };
        private GUIContent label;
        private ConstructorInfo constructor;
        private MethodInfo parser;

        private PanoID Create(string value)
        {
            var args = new[] { value };
            if (constructor != null)
            {
                return (PanoID)constructor.Invoke(args);
            }
            else if (parser != null)
            {
                return (PanoID)parser.Invoke(null, args);
            }
            else
            {
                return default;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (this.label == null)
            {
                this.label = new GUIContent("Pano ID", "Google Street View persistent panoramic image ID.");
                var t = typeof(PanoID);
                constructor = t.GetConstructor(paramTypes);
                parser = t.GetMethod("Parse", paramTypes);
            }

            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;

            try
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                EditorGUI.indentLevel = 0;

                if (property.isArray)
                {
                    EditorGUI.PropertyField(position, property, false);
                }
                else
                {
                    EditorGUIUtility.labelWidth = 25;
                    var rect = new Rect(position.x, position.y, position.width, position.height);
                    var curValue = property.GetValue<PanoID>();
                    curValue = Create(EditorGUI.TextField(rect, this.label, curValue.ToString()));
                    property.SetValue(curValue);
                }

            }
            finally
            {
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
            }
        }
    }
}