using System.Linq;

namespace UnityEngine
{
    public static class TagManager
    {
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

        public static void Normalize(string tag)
        {
#if UNITY_EDITOR
            if (!DEFAULT_TAGS.Contains(tag))
            {
                var tagManager = new UnityEditor.SerializedObject(ComponentExt.EditorLoadAllAssets("ProjectSettings/TagManager.asset")[0]);
                var tagsProp = tagManager.FindProperty("tags");
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize && !found; i++)
                {
                    var t = tagsProp.GetArrayElementAtIndex(i);
                    found = t.stringValue == tag;
                }

                if (!found)
                {
                    var n = tagsProp.arraySize;
                    tagsProp.InsertArrayElementAtIndex(n);
                    var tagProp = tagsProp.GetArrayElementAtIndex(n);
                    tagProp.stringValue = tag;
                    tagManager.ApplyModifiedProperties();
                }
            }
#endif
        }
    }
}
