using System;
using System.Linq;

using Juniper.Input;

using UnityEditor;

using UnityEngine;

namespace Juniper.Events
{
    [CustomEditor(typeof(ButtonEvent))]
    public class ButtonEventEditor : UnityEditor.Editor
    {
        private static readonly ButtonEvent _;
        private const string FIELD_ONCLICK = nameof(_.onClick);
        private const string FIELD_ONDOUBLECLICK = nameof(_.onDoubleClick);
        private const string FIELD_ONLONGPRESS = nameof(_.onLongPress);
        private const string FIELD_ONDOWN = nameof(_.onDown);
        private const string FIELD_ONUP = nameof(_.onUp);

        private static readonly GUIContent ButtonTypeLabel = new GUIContent("Button Type");
        private static readonly GUIContent ButtonValueLabel = new GUIContent("Button Value");
        private static readonly GUIContent InputKeyLabel = new GUIContent("Input Key");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var value = (ButtonEvent)serializedObject.targetObject;
            var enumTypes = value.GetSupportedButtonTypes().ToArray();
            var enumTypeNames = enumTypes.Select(t => t.FullName).ToArray();
            var enumTypeLabels = enumTypeNames.Select(n => new GUIContent(n)).ToArray();
            var selectedTypeIndex = ArrayUtility.IndexOf(enumTypeNames, value.buttonTypeName);
            selectedTypeIndex = EditorGUILayout.Popup(ButtonTypeLabel, selectedTypeIndex, enumTypeLabels);
            var destroy = false;
            if (0 <= selectedTypeIndex)
            {
                value.buttonTypeName = enumTypeNames[selectedTypeIndex];
                var enumType = enumTypes[selectedTypeIndex];
                var enumStrings = Enum.GetNames(enumType);
                var enumLabels = enumStrings.Select(n => new GUIContent(n)).ToArray();
                var selectedValueIndex = ArrayUtility.IndexOf(enumStrings, value.buttonValueName);
                selectedValueIndex = EditorGUILayout.Popup(ButtonValueLabel, selectedValueIndex, enumLabels);

                if (0 > selectedValueIndex)
                {
                    value.buttonValueName = null;
                }
                else
                {
                    var buttonValueName = enumStrings[selectedValueIndex];
                    var key = ButtonEvent.FormatKey(value.buttonTypeName, buttonValueName);
                    var matching = value.GetComponents<ButtonEvent>()
                        .Count(e => e.Key == key && e != value);
                    if (matching <= 0)
                    {
                        value.buttonValueName = buttonValueName;
                        value.inputKey = (KeyCode)EditorGUILayout.EnumPopup(InputKeyLabel, value.inputKey);
                    }
                    else if (EditorUtility.DisplayDialog("Error", $"A ButtonEvent for {key} already exists. Do you want to delete this ButtonEvent? If you keep this ButtonEvent, its Button Value will be reverted to its previous value.", "Delete", "Keep"))
                    {
                        destroy = true;
                    }
                }
            }

            if (destroy)
            {
                value.DestroyImmediate();
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(FIELD_ONDOWN));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(FIELD_ONCLICK));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(FIELD_ONDOUBLECLICK));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(FIELD_ONLONGPRESS));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(FIELD_ONUP));

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
