using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine
{
    public static class ResourceExt
    {

#if UNITY_EDITOR

        /// <summary>
        /// When called from the Unity Editor, loads an asset from disk to use as a property in
        /// a Unity component.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T EditorLoadAsset<T>(string path) where T : Object
        {
            var fixedPath = System.IO.PathExt.FixPath(path);
            if (System.IO.File.Exists(fixedPath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fixedPath);
            }
            else
            {
                return UnityEditor.AssetDatabase.GetBuiltinExtraResource<T>(path);
            }
        }

        /// <summary>
        /// When called from the Unity Editor, loads un-typed assets that can be inspected
        /// as SerializedObjects.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<UnityEditor.SerializedObject> EditorLoadAllAssets(string path)
        {
            return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(System.IO.PathExt.FixPath(path))
                .Select(obj => new UnityEditor.SerializedObject(obj));
        }

        /// <summary>
        /// When called from the Unity Editor, loads an un-typed asset that can be inspected
        /// as a SerializedObject.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEditor.SerializedObject EditorLoadAsset(string path)
        {
            return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(System.IO.PathExt.FixPath(path))
                .Select(obj => new UnityEditor.SerializedObject(obj))
                .FirstOrDefault();
        }

#endif

        /// <summary>
        /// When called from the Unity Editor, loads an audio clip from disk to use as a property in
        /// a Unity component.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string path) where T : Object
        {
            return Resources.Load<T>(System.IO.PathExt.FixPath(path));
        }
    }
}
