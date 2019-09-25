using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper;

namespace UnityEngine
{
    /// <summary>
    /// Extensions to Unity's GameObject class.
    /// </summary>
    public static class GameObjectExt
    {
        /// <summary>
        /// Set the active status for a game object and all of its parent objects in the scene graph.
        /// </summary>
        /// <param name="obj">   Object.</param>
        /// <param name="active">If set to <c>true</c> active.</param>
        public static void SetTreeActive(this GameObject obj, bool active)
        {
            var head = obj.transform;
            while (head != null && !head.IsActivated())
            {
                head.SetActive(active);
                head = head.transform.parent;
            }
        }

        /// <summary>
        /// Activate a game object and all of its parent objects in the scene graph.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void Activate(this GameObject obj)
        {
            obj.SetActive(true);
        }

        /// <summary>
        /// Deactivate a game object and all of its parent objects in the scene graph.
        /// </summary>
        /// <param name="obj">Object.</param>
        public static void Deactivate(this GameObject obj)
        {
            obj.SetActive(false);
        }

        public static void DestroyImmediate(this GameObject obj)
        {
            Object.DestroyImmediate(obj);
        }

        public static void Destroy(this GameObject obj)
        {
            Object.Destroy(obj);
        }

        /// <summary>
        /// Check to see if a particular game object is active and in the scene.
        /// </summary>
        /// <returns><c>true</c>, if activated was ised, <c>false</c> otherwise.</returns>
        /// <param name="parent">Parent.</param>
        public static bool IsActivated(this GameObject parent)
        {
            return parent?.activeInHierarchy == true;
        }

        /// <summary>
        /// Attempts to get a component of type <typeparamref name="T"/> from <paramref name="obj"/>.
        /// If one is not available, this function creates one on the gameObject and returns that.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="obj">Object.</param>
        /// <typeparam name="T">A subclass of type <see cref="Component"/>.</typeparam>
        public static PooledComponent<T> Ensure<T>(this GameObject obj, Predicate<T> predicate) where T : Component
        {
            return new PooledComponent<T>(obj, predicate, null);
        }

        public static PooledComponent<T> Ensure<T>(this GameObject obj) where T : Component
        {
            return new PooledComponent<T>(obj, null, null);
        }

        /// <summary>
        /// Concatenate the name of all of the objects in the scene graph hierarchy above this
        /// object, to create a path-like view of where the object is located.
        /// </summary>
        /// <returns>The name.</returns>
        /// <param name="obj">Object.</param>
        public static string FullName(this GameObject obj)
        {
            var parts = new List<string>(5);
            var tail = obj.transform;
            while (tail != null)
            {
                parts.Add(tail.name);
                tail = tail.parent;
            }

            parts.Reverse();

            return parts.ToArray().Join();
        }

        /// <summary>
        /// Take two game objects and figure out the relative scene navigation path between them.
        /// </summary>
        /// <returns>The name.</returns>
        /// <param name="to">  To.</param>
        /// <param name="from">From.</param>
        public static string RelativeName(this GameObject to, GameObject from)
        {
            var fromName = from.FullName();
            var toName = to.FullName();
            var fromParts = fromName
                .NormalizePath()
                .Split(Path.DirectorySeparatorChar)
                .ToList();
            var toParts = toName
                .NormalizePath()
                .Split(Path.DirectorySeparatorChar)
                .ToList();

            while (fromParts.Count > 0 && toParts.Count > 0 && fromParts[0] == toParts[0])
            {
                fromParts.RemoveAt(0);
                toParts.RemoveAt(0);
            }

            var parts = new List<string>(5);
            foreach (var part in fromParts)
            {
                parts.Add("..");
            }

            foreach (var part in toParts)
            {
                parts.Add(part);
            }

            return parts.ToArray().Join();
        }

        /// <summary>
        /// Turn off any renderers on the current game object, and deactivate all children in the transform.
        /// </summary>
        /// <param name="t">      T.</param>
        /// <param name="visible">If set to <c>true</c> visible.</param>
        public static void SetVisible(this GameObject t, bool visible)
        {
            if (t != null)
            {
                t.transform.SetVisible(visible);
            }
        }

        /// <summary>
        /// Figures out a rough "center" location for an object that might include a mesh renderer,
        /// or children with a mesh renderer.
        /// </summary>
        /// <returns>The center.</returns>
        /// <param name="obj">    Object.</param>
        /// <param name="exclude">Exclude.</param>
        public static Vector3 Center(this GameObject obj, params Component[] exclude)
        {
            var output = Vector3.zero;
            var count = 0;
            var excludeTrans = from e in exclude select e.transform;
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            {
                if (!excludeTrans.Contains(renderer.transform) && !excludeTrans.Contains(renderer.transform.parent))
                {
                    ++count;
                    if (renderer.bounds.Volume() > 0)
                    {
                        output += renderer.bounds.center;
                    }
                    else
                    {
                        output += renderer.transform.position;
                    }
                }
            }

#if UNITY_MODULES_UI
            foreach (var renderer in obj.GetComponentsInChildren<CanvasRenderer>())
            {
                var rect = renderer.GetComponent<RectTransform>();
                if (!excludeTrans.Contains(rect) && !excludeTrans.Contains(rect.parent))
                {
                    ++count;
                    output += rect.position;
                    if (rect.rect.width > 0)
                    {
                        output.x += (rect.rect.xMax + rect.rect.xMin) / 2;
                    }
                    if (rect.rect.height > 0)
                    {
                        output.y += (rect.rect.yMax + rect.rect.yMin) / 2;
                    }
                }
            }
#endif

            output /= count;
            return output;
        }

        /// <summary>
        /// Checks to see if a gameObject has a particular component and, if it does, destroys it.
        /// </summary>
        /// <returns><c>true</c>, if component existed to be destroy, <c>false</c> otherwise.</returns>
        /// <param name="obj">The gameObject from which to remove the component.</param>
        /// <typeparam name="T">A subclass of type <see cref="Component"/>.</typeparam>
        public static bool Remove<T>(this GameObject obj) where T : Component
        {
            var o = obj.GetComponent<T>();
            if (o != null)
            {
                o.DestroyImmediate();
            }
            return o != null;
        }
    }
}
