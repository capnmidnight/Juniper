using Juniper.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
    /// <summary>
    /// Extension methods for Unity's RectTransform class.
    /// </summary>
    public static class RectTransformExt
    {
        /// <summary>
        /// Resizes a RectTransform so that all of its children are visible within.
        /// </summary>
        /// <param name="parent">Parent.</param>
        public static void ResizeContentArea(this RectTransform parent)
        {
            var lastChild = parent.GetChild(parent.childCount - 1).GetComponent<RectTransform>();
            var contentPanel = parent.GetComponent<RectTransform>();
            contentPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -lastChild.anchoredPosition.y);
        }

        public static T SetAnchors<T>(this T parent, Vector2 anchorMin, Vector2 anchorMax)
            where T : Component
        {
            var rect = parent.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            return parent;
        }

        public static T SetPivot<T>(this T parent, Vector2 pivot)
            where T : Component
        {
            var rect = parent.GetComponent<RectTransform>();
            rect.pivot = pivot;
            return parent;
        }

        public static T SetPosition<T>(this T parent, Vector3 position)
            where T : Component
        {
            var rect = parent.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            return parent;
        }

        public static T SetWidth<T>(this T parent, float width)
            where T : Component
        {
            var rect = parent.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            return parent;
        }

        public static T SetHeight<T>(this T parent, float height)
            where T : Component
        {
            var rect = parent.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            return parent;
        }

        public static T SetSize<T>(this T parent, float width, float height)
            where T : Component
        {
            return parent.SetWidth(width)
                .SetHeight(height);
        }

        public static T SetSize<T>(this T parent, Vector2 sizeDelta)
            where T : Component
        {
            var rect = parent.GetComponent<RectTransform>();
            rect.sizeDelta = sizeDelta;
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
        public static T Query<T>(this RectTransform parent, string path)
        {
            var parts = path.Split('/');
            var top = new List<RectTransform> { parent };
            for (var i = 0; i < parts.Length; ++i)
            {
                var part = parts[i];
                if (part != ".")
                {
                    var next = new List<RectTransform>();

                    foreach (var here in top)
                    {
                        if (here == null)
                        {
                            var root = (from scene in JuniperPlatform.AllScenes
                                        from gameObject in scene.GetRootGameObjects()
                                        where gameObject.name == part
                                        select gameObject.GetComponent<RectTransform>())
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
                            next.Add(here.gameObject.transform.parent.GetComponent<RectTransform>());
                        }
                        else if (part.Length > 0)
                        {
                            foreach (var child in here.Children())
                            {
                                if (child?.name == part)
                                {
                                    next.Add(child);
                                }
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

            top.RemoveAll(x => x == null);

            if (top.Count > 0)
            {
                return top[0].GetComponent<T>();
            }
            else
            {
                return default(T);
            }
        }

        public static RectTransform RectQuery<T>(this T parent, string path)
            where T : Component
        {
            return parent.Query<RectTransform>(path);
        }

        public static PooledComponent<RectTransform> EnsureRectTransform<T>(this T obj, string path, string creationPath = null)
            where T : Component
        {
            if (creationPath == null)
            {
                creationPath = path;
            }

            var rect = obj.EnsureComponent<RectTransform>();
            var trans = rect.RectQuery(path);
            var isNew = trans == null;
            if (isNew)
            {
                var parts = creationPath.Split('/');
                var name = parts.LastOrDefault();
                trans = new GameObject(name).EnsureComponent<RectTransform>();
                RectTransform parent;
                if (parts.Length == 1)
                {
                    parent = obj.GetComponent<RectTransform>();
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
                    parent = obj.EnsureRectTransform(parentPath);
                }

                trans.SetParent(parent, false);
            }

            return new PooledComponent<RectTransform>(trans, isNew);
        }

        /// <summary>
        /// Returns true if a point is contained within the rectangle.
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="point">Point.</param>
        public static bool Contains(this RectTransform parent, Vector2 point)
        {
            if (parent == null)
            {
                return false;
            }
            else
            {
                var p = parent.position;
                var s = parent.sizeDelta;
                var minX = p.x;
                var maxX = p.x + s.x;
                var minY = p.y;
                var maxY = p.y + s.y;
                return minX <= point.x
                    && point.x < maxX
                    && minY <= point.y
                    && point.y < maxY;
            }
        }
    }
}
