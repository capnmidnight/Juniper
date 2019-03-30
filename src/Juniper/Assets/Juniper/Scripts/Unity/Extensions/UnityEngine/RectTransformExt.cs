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
        /// Returns true if a point is contained within the rectangle.
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="point"> Point.</param>
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