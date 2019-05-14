using System.Reflection;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

namespace Juniper
{
    public static class SerializedPropertyExt
    {
        public static T GetValue<T>(this SerializedProperty property) where T : struct
        {
            var value = GetObject(property);
            if (value == null)
            {
                return default;
            }
            else
            {
                return (T)value;
            }
        }

        public static T GetObject<T>(this SerializedProperty property) where T : new()
        {
            return (T)GetObject(property);
        }

        public static T GetScriptableObject<T>(this SerializedProperty property) where T : ScriptableObject
        {
            var obj = GetObject(property);
            if (obj == null)
            {
                return ScriptableObject.CreateInstance<T>();
            }
            else
            {
                return (T)obj;
            }
        }

        /// <summary>
        /// The array index pattern.
        /// </summary>
        private static readonly Regex arrayIndexPattern = new Regex("data\\[(\\w+)\\]", RegexOptions.Compiled);

        private static object GetObject(SerializedProperty property)
        {
            object head = property.serializedObject.targetObject;
            var parts = property.propertyPath.Split('.');
            for (var i = 0; i < parts.Length && head != null; ++i)
            {
                var part = parts[i];
                if (part == "Array")
                {
                    var indexStr = parts[++i];
                    var match = arrayIndexPattern.Match(indexStr);
                    var index = int.Parse(match.Groups[1].Value);
                    var arr = (object[])head;
                    if (0 <= index && index < arr.Length)
                    {
                        head = arr[index];
                    }
                    else
                    {
                        head = null;
                    }
                }
                else
                {
                    var type = head.GetType();
                    var field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    head = field.GetValue(head);
                }
            }

            return head;
        }
    }
}
