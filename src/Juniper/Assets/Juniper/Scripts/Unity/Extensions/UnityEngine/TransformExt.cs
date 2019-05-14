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
        /// <param name="t">         </param>
        /// <param name="parent">    </param>
        /// <param name="resetScale">
        /// Set to false to keep the object at its original size. Set to true to reset the scale to
        /// the unity scale. Defaults to true.
        /// </param>
        public static void Reparent(this Transform t, Transform parent, bool resetScale = true)
        {
            t.SetParent(parent, !resetScale);
            t.Reset(resetScale);
        }

        /// <summary>
        /// Resets the transform's localPosition, localRotation, and localScale to the origin state.
        /// </summary>
        /// <param name="t">         </param>
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

        /// <summary>
        /// Enumerate through a transform and all of its children, as if they were one collection.
        /// </summary>
        /// <returns>The family.</returns>
        /// <param name="parent">Parent.</param>
        public static IEnumerable<T> FamilyTree<T>(this T parent)
            where T : Transform
        {
            var q = new Queue<T>(10) { parent };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(here.Children());
                yield return here;
            }
        }

        /// <summary>
        /// Remove all child transforms from a transform.
        /// </summary>
        /// <param name="parent">    </param>
        /// <param name="startIndex"></param>
        public static Transform ClearChildren(this Transform parent, int startIndex = 0)
        {
            for (var i = parent.childCount - 1; i >= startIndex; --i)
            {
                parent.GetChild(i).gameObject.Destroy();
            }
            return parent;
        }

        /// <summary>
        /// Search through a series of Transforms and child transforms, defined as a set of
        /// forward-slash delimited names. Use ".." to select the parent transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="path">  </param>
        /// <returns></returns>
        public static Transform Query<T>(this T parent, string path)
            where T : Component
        {
            var parts = path.Split('/');
            var top = new List<Transform>(5) { parent.transform };
            for (var i = 0; i < parts.Length; ++i)
            {
                var part = parts[i];
                if (part != ".")
                {
                    var next = new List<Transform>(5);

                    foreach (var here in top)
                    {
                        if (here == null)
                        {
                            next.Add((from scene in JuniperPlatform.AllScenes
                                      from gameObject in scene.GetRootGameObjects()
                                      where gameObject.name == part
                                      select gameObject.transform)
                                .FirstOrDefault());
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
                        else
                        {
                            next.Add(null);
                        }
                    }

                    if (next.Count == 0 && i < parts.Length - 1)
                    {
                        return null;
                    }
                    else
                    {
                        top = next;
                    }
                }
            }

            top.RemoveAll(x => x == null);

            if (top.Count > 0)
            {
                return top[0].transform;
            }
            else
            {
                return null;
            }
        }

        public static T SetScale<T>(this T t, Vector3 s)
            where T : Component
        {
            if (t != null)
            {
                t.transform.localScale = s;
            }
            return t;
        }

        public static void Destroy(this Transform t)
        {
            t.gameObject.Destroy();
        }
    }
}
