using System;
using System.Collections.Generic;
using System.Linq;

using Juniper;

namespace UnityEngine
{
    /// <summary>
    /// Extensions to Unity's Transform class.
    /// </summary>
    public static class TransformExt
    {
        /// <summary>
        /// Move the transform onto a parent, and reset it's localPosition, localRotation, and
        /// localScale to the origin state.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parent"></param>
        /// <param name="resetScale">
        /// Set to false to keep the object at its original size. Set to true to reset the scale to
        /// the unity scale. Defaults to true.
        /// </param>
        public static void Reparent(this Transform t, Transform parent, bool resetScale = true)
        {
            t.SetParent(parent, !resetScale);
            t.Reset(resetScale);
        }

        public static void Reparent(this PooledComponent<Transform> t, Transform parent, bool resetScale = true) =>
            t.Value.Reparent(parent, resetScale);

        /// <summary>
        /// Resets the transform's localPosition, localRotation, and localScale to the origin state.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="resetScale">
        /// Set to false to keep the object at its original size. Set to true to reset the scale to
        /// the unity scale. Defaults to true.
        /// </param>
        public static void Reset(this Transform t, bool resetScale = true)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            if (resetScale)
            {
                t.localScale = Vector3.one;
            }
        }

        public static void Reset(this PooledComponent<Transform> t, bool resetScale = true) =>
            t.Value.Reset(resetScale);

        /// <summary>
        /// Enumerate through all of the transform's children.
        /// </summary>
        /// <returns>The children.</returns>
        /// <param name="parent">Parent.</param>
        public static IEnumerable<T> Children<T>(this T parent)
            where T : Transform
        {
            for (var i = 0; i < parent.childCount; ++i)
            {
                yield return parent.GetChild(i).GetComponent<T>();
            }
        }

        public static IEnumerable<T> Children<T>(this PooledComponent<T> parent)
            where T : Transform =>
            parent.Value.Children();

        /// <summary>
        /// Enumerate through a transform and all of its children, as if they were one collection.
        /// </summary>
        /// <returns>The family.</returns>
        /// <param name="parent">Parent.</param>
        public static IEnumerable<T> Family<T>(this T parent)
            where T : Transform
        {
            yield return parent;
            foreach (var child in parent.Children())
            {
                yield return child;
            }
        }

        public static IEnumerable<T> Family<T>(this PooledComponent<T> parent)
            where T : Transform =>
            parent.Value.Family();

        /// <summary>
        /// Remove all child transforms from a transform.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="startIndex"></param>
        public static Transform ClearChildren(this Transform parent, int startIndex = 0)
        {
            for (var i = parent.childCount - 1; i >= startIndex; --i)
            {
                parent.GetChild(i).gameObject.Destroy();
            }
            return parent;
        }

        public static PooledComponent<Transform> ClearChildren(this PooledComponent<Transform> parent, int startIndex = 0)
        {
            parent.Value.ClearChildren(startIndex);
            return parent;
        }

        /// <summary>
        /// Search through a series of Transforms and child transforms, defined as a set of
        /// forward-slash delimited names. Use ".." to select the parent transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Query<T>(this Transform parent, string path)
        {
            var parts = path.Split('/');
            var top = new List<Transform> { parent };
            for (var i = 0; i < parts.Length; ++i)
            {
                var part = parts[i];
                if (part != ".")
                {
                    var next = new List<Transform>();

                    foreach (var here in top)
                    {
                        if (here == null)
                        {
                            var root = (from scene in MasterSceneController.AllScenes
                                        from gameObject in scene.GetRootGameObjects()
                                        where gameObject.name == part
                                        select gameObject.transform)
                                .FirstOrDefault();
                            if (root == null)
                            {
                                throw new ArgumentException("Root object " + root + " does not exist");
                            }
                            else
                            {
                                next.Add(root);
                            }
                        }
                        else if (part == "*")
                        {
                            next.AddRange(here.Children());
                        }
                        else if (part == "..")
                        {
                            next.Add(here.parent);
                        }
                        else if (part.Length > 0)
                        {
                            var child = here.Find(part);
                            if (child != null)
                            {
                                next.Add(child);
                            }
                        }
                    }

                    if (next.Count == 0 && i < parts.Length - 1)
                    {
                        return default(T);
                    }
                    else
                    {
                        top = next;
                    }
                }
            }

            top.RemoveAll(x =>
                x == null);

            if (top.Count > 0)
            {
                return top[0].GetComponent<T>();
            }
            else
            {
                return default(T);
            }
        }

        public static T Query<T>(this PooledComponent<Transform> parent, string path) =>
            parent.Value.Query<T>(path);

        /// <summary>
        /// Search through a series of Transforms and child transforms, defined as a set of
        /// forward-slash delimited names. Use ".." to select the parent transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Transform Query(this Transform parent, string path) =>
            parent.Query<Transform>(path);

        public static Transform Query(this PooledComponent<Transform> parent, string path) =>
            parent.Value.Query<Transform>(path);

        /// <summary>
        /// Search the given transform's children and their children's children recursively until we
        /// find a gameObject of the given name.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform Search(this Transform parent, string name)
        {
            Transform child = null;

            var searchSpace = new Queue<Transform>();
            searchSpace.Enqueue(parent);

            while (searchSpace.Count > 0)
            {
                var here = searchSpace.Dequeue();
                child = here.Find(name);
                if (child != null)
                {
                    break;
                }
                else
                {
                    foreach (Transform sub in here)
                    {
                        searchSpace.Enqueue(sub);
                    }
                }
            }

            return child;
        }

        public static Transform Search(this PooledComponent<Transform> parent, string name) =>
            parent.Value.Search(name);

        /// <summary>
        /// Get a component from a transform, creating it if it doesn't exist.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="obj">Object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static PooledComponent<T> EnsureComponent<T>(this Transform obj)
            where T : Component =>
            obj.gameObject.EnsureComponent<T>();

        public static PooledComponent<T> EnsureComponent<T>(this PooledComponent<Transform> obj)
            where T : Component =>
            obj.Value.gameObject.EnsureComponent<T>();

        public static T SetScale<T>(this T t, Vector3 s)
            where T : Transform
        {
            if (t != null)
            {
                t.localScale = s;
            }
            return t;
        }

        public static PooledComponent<Transform> SetScale(this PooledComponent<Transform> t, Vector3 s)
        {
            t.Value.SetScale(s);
            return t;
        }
    }
}
