using UnityEngine;

namespace UnityEditor
{
    public static class StringExt
    {
        public static GUIContent ToGUIContent(this string value)
        {
            return new GUIContent(value);
        }

        public static GUIContent[] ToGUIContents(this string[] values)
        {
            var arr = new GUIContent[values.Length];
            for(int i = 0; i < values.Length; ++i)
            {
                arr[i] = values[i].ToGUIContent();
            }

            return arr;
        }
    }
}
