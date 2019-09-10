using System.Linq;
using UnityEngine;

namespace Juniper.ConfigurationManagement
{
    public static class TagManager
    {
#if UNITY_EDITOR

        private static readonly string[] DEFAULT_TAGS = new string[]
        {
            "Untagged",
            "Respawn",
            "Finish",
            "EditorOnly",
            "MainCamera",
            "Player",
            "GameController"
        };

        private static readonly string[] DEFAULT_LAYERS = new string[]
        {
            "Default",
            "TransparentFX",
            "Ignore Raycast",
            "EditorOnly",
            "Water",
            "UI"
        };

        private static void MaybeAddArrayElement(string arrayName, string[] defaultValues, string value)
        {
            if (!defaultValues.Contains(value))
            {
                var tagManager = ResourceExt.EditorLoadAsset("ProjectSettings/TagManager.asset");
                var arrayProp = tagManager.FindProperty(arrayName);
                bool found = false;
                for (int i = 0; i < arrayProp.arraySize && !found; i++)
                {
                    var arrayElement = arrayProp.GetArrayElementAtIndex(i);
                    found = arrayElement.stringValue == value;
                }

                if (!found)
                {
                    var n = arrayProp.arraySize;
                    arrayProp.InsertArrayElementAtIndex(n);
                    var newAarrayElement = arrayProp.GetArrayElementAtIndex(n);
                    newAarrayElement.stringValue = value;
                    tagManager.ApplyModifiedProperties();
                }
            }
        }

#endif

        public static void NormalizeTag(string tag)
        {
#if UNITY_EDITOR
            MaybeAddArrayElement("tags", DEFAULT_TAGS, tag);
#endif
        }

        public static void NormalizeLayer(string layer)
        {
#if UNITY_EDITOR
            MaybeAddArrayElement("layers", DEFAULT_LAYERS, layer);
#endif
        }
    }
}