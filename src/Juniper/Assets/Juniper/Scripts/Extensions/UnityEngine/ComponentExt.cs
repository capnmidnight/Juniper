using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Juniper;
using Juniper.World;

namespace UnityEngine
{
    /// <summary>
    /// Extension methods for Unity's Component class.
    /// </summary>
    public static class ComponentExt
    {
        /// <summary>
        /// ARCore looks for certain things as soon as the component is enabled, so we have to delay
        /// it. First, we disable the object hierarchy in question. Then, we execute the code to
        /// create the ARCore objects. Finally, we re-enable the hierarchy.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="act"></param>
        public static void WithLock(this Component src, Action act)
        {
            src.Deactivate();
            act();
            src.Activate();
        }

        /// <summary>
        /// Check to see if a component is valid (not-null) and activated.
        /// </summary>
        /// <param name="parent">The component to check</param>
        public static bool IsActivated(this Component parent)
        {
            return parent.gameObject.IsActivated();
        }

        /// <summary>
        /// Set the active state for the whole hierarchy from an object on up through its parent
        /// transforms. Useful for making absolutely sure a particular object is activated.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="active">If set to <c>true</c> active.</param>
        public static void SetTreeActive(this Component parent, bool active)
        {
            parent.gameObject.SetTreeActive(active);
        }

        /// <summary>
        /// Set the active state for the GameObject of any particular Component. Just a useful shortcut.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="active">If set to <c>true</c> active.</param>
        public static void SetActive(this Component parent, bool active)
        {
            parent.gameObject.SetActive(active);
        }

        /// <summary>
        /// A shortcut for
        /// <code>
        /// SetActive(true)
        /// </code>
        /// . Also useful for feeding to higher-order functions that expect parameterless functions.
        /// </summary>
        /// <param name="parent">Parent.</param>
        public static void Activate(this Component parent)
        {
            parent.gameObject.Activate();
        }

        /// <summary>
        /// A shortcut for
        /// <code>
        /// SetActive(false)
        /// </code>
        /// . Also useful for feeding to higher-order functions that expect parameterless functions.
        /// </summary>
        /// <param name="parent">Parent.</param>
        public static void Deactivate(this Component parent)
        {
            parent.gameObject.Deactivate();
        }

        public static void DestroyImmediate(this Object obj)
        {
            Object.DestroyImmediate(obj);
        }

        public static void Destroy(this Object obj)
        {
            Object.Destroy(obj);
        }

        public static T Query<T>(this Component parent, string path)
        {
            var trans = parent.Query(path);
            if (trans == null)
            {
                return default;
            }
            else
            {
                return trans.GetComponent<T>();
            }
        }

        /// <summary>
        /// Search through a series of Transforms and child transforms, defined as a set of
        /// forward-slash delimited names. Use ".." to select the parent transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="path">  </param>
        /// <returns></returns>
        public static Transform Query(this Component parent, string path)
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
                            next.Add((from scene in JuniperSystem.AllScenes
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

        /// <summary>
        /// Check to see if an object is associated with an active renderer.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the Component has a sibling Compoent that is an enabled Renderer,
        /// <c>false</c> otherwise.
        /// </returns>
        /// <param name="parent">Parent.</param>
        public static bool GetVisible(this Component parent)
        {
            var r = parent.GetComponent<Renderer>();
            return r != null
                && r.enabled;
        }

        /// <summary>
        /// For any renderers on the <paramref name="parent"/>, sets its enabled property to
        /// <paramref name="visible"/>. For any child transforms, calls <see
        /// cref="SetActive(Component, bool)"/> with <paramref name="visible"/> as the value.
        /// </summary>
        /// <param name="parent"> </param>
        /// <param name="visible"></param>
        public static void SetVisible(this Component parent, bool visible)
        {
            var r = parent.GetComponent<Renderer>();
            if (r != null)
            {
                r.enabled = visible;
            }

            foreach (Transform child in parent.transform)
            {
                child.SetActive(visible);
            }
        }

        public static bool HasComponent<T>(this Component parent)
        {
            return parent.GetComponent<T>() != null;
        }

        /// <summary>
        /// Attempts to get a component of type <typeparamref name="T"/> from <paramref name="obj"/>.
        /// If one is not available, this function creates one on the gameObject and returns that.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="obj">Object.</param>
        /// <typeparam name="T">A subclass of type <see cref="Component"/>.</typeparam>
        public static PooledComponent<T> Ensure<T>(this Component obj, Predicate<T> predicate) where T : Component
        {
            return obj.gameObject.Ensure(predicate);
        }

        public static PooledComponent<T> Ensure<T>(this Component obj)
            where T : Component
        {
            return obj.gameObject.Ensure<T>();
        }

        public static PooledComponent<Transform> EnsureParent(this Component obj, string name, Transform skip)
        {
            var parent = obj.transform.parent;
            if (parent == null || parent == skip)
            {
                parent = new GameObject(name).transform;
                obj.transform.SetParent(parent, false);
                return new PooledComponent<Transform>(parent, true);
            }
            else
            {
                parent.name = name;
                return new PooledComponent<Transform>(parent, false);
            }
        }

        /// <summary>
        /// Checks to see if a gameObject has a particular component and, if it does, destroys it.
        /// </summary>
        /// <returns><c>true</c>, if component existed to be destroy, <c>false</c> otherwise.</returns>
        /// <param name="obj">The gameObject from which to remove the component.</param>
        /// <typeparam name="T">A subclass of type <see cref="Component"/>.</typeparam>
        public static bool Remove<T>(this Component obj) where T : Component
        {
            var o = obj.GetComponent<T>();
            if (o != null)
            {
                o.DestroyImmediate();
            }
            return o != null;
        }

        /// <summary>
        /// Fill in a path of Transforms with other Transforms as necessary. Useful for creating
        /// complex Transform hierarchies very quickly.
        /// </summary>
        /// <returns>The transform.</returns>
        /// <param name="obj">         Object.</param>
        /// <param name="path">        Path.</param>
        /// <param name="creationPath"></param>
        /// <param name="create">      </param>
        public static PooledComponent<T> Ensure<T>(this Component obj, string path, string creationPath, Func<GameObject> create)
            where T : Transform
        {
            if (creationPath == null)
            {
                creationPath = path;
            }

            var trans = obj.transform.Query(path);
            var isNew = trans == null;
            if (isNew)
            {
                var parts = creationPath.Split('/');
                var name = parts.LastOrDefault();

                if (create == null)
                {
                    create = () => new GameObject();
                }

                trans = create().transform;
                trans.name = name;

                Transform parent;
                if (parts.Length == 1)
                {
                    parent = obj.transform;
                }
                else if (parts.Length == 0 || (parts.Length == 2 && parts[0].Length == 0))
                {
                    parent = null;
                }
                else
                {
                    var parentPath = string.Join("/", parts
                        .Take(parts.Length - 1)
                        .ToArray());
                    parent = obj.Ensure<T>(parentPath);
                }

                trans.SetParent(parent, false);
            }

            return new PooledComponent<T>(trans.gameObject, isNew);
        }

        public static PooledComponent<T> Ensure<T>(this Component obj, string path, string creationPath)
            where T : Transform
        {
            return obj.Ensure<T>(path, creationPath, null);
        }

        /// <summary>
        /// Fill in a path of Transforms with other Transforms as necessary. Useful for creating
        /// complex Transform hierarchies very quickly.
        /// </summary>
        /// <returns>The transform.</returns>
        /// <param name="obj">   Object.</param>
        /// <param name="path">  Path.</param>
        /// <param name="create"></param>
        public static PooledComponent<T> Ensure<T>(this Component obj, string path, Func<GameObject> create)
            where T : Transform
        {
            return obj.Ensure<T>(path, null, create);
        }

        public static PooledComponent<T> Ensure<T>(this Component obj, string path)
            where T : Transform
        {
            return obj.Ensure<T>(path, null, null);
        }

        public static string GetRelativeName(this Component child, Component root)
        {
            var parent = child.transform.parent;
            string transName = child.name;
            while (parent != root.transform)
            {
                transName = parent.name + "/" + transName;
                parent = parent.parent;
            }

            return transName;
        }

        public static string GetZoneName(this Component child)
        {
            var here = child.transform;
            while (here != null)
            {
                var zone = here.GetComponent<Zone>();
                if (zone != null)
                {
                    return zone.zoneName;
                }

                here = here.parent;
            }

            return null;
        }

        public static bool IsInAnyZone(this Component child)
        {
            return child.GetComponentInParent<Zone>() != null;
        }

        public static bool IsInZone(this Component child, string zoneName)
        {
            var here = child.transform;
            while (here != null)
            {
                var zones = here.GetComponents<Zone>();
                foreach (var zone in zones)
                {
                    if (zone.zoneName == zoneName)
                    {
                        return true;
                    }
                }

                here = here.parent;
            }

            return false;
        }
    }
}