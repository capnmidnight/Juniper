namespace UnityEngine
{
    public static class CameraExt
    {
        /// <summary>
        /// Determine if a target point is within the field of view of the camera.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsInView(this Camera cam, Vector3 target)
        {
            var viewport = cam.WorldToViewportPoint(target);
            return 0 <= viewport.x && viewport.x < 1
                && 0 < viewport.y && viewport.y < 1;
        }


#if UNITY_MODULES_UI
        private static readonly Vector3[] corners = new Vector3[4];

        public static bool IsInView(this Camera cam, Canvas target)
        {
            if (target?.isActiveAndEnabled != true)
            {
                return false;
            }
            else if (target.renderMode == RenderMode.WorldSpace)
            {
                return true;
            }
            else
            {
                var rect = target.GetComponent<RectTransform>();
                rect.GetWorldCorners(corners);

                var ab = corners[1] - corners[0];
                var bc = corners[2] - corners[1];
                var cd = corners[3] - corners[2];
                var da = corners[0] - corners[3];

                var normal = Vector3.Cross(ab, bc);
                var f = cam.transform.forward + normal;

                var a = Vector3.Dot(ab, f);
                var b = Vector3.Dot(bc, f);
                var c = Vector3.Dot(cd, f);
                var d = Vector3.Dot(da, f);

                return 0 <= a
                    && 0 <= b
                    && 0 <= c
                    && 0 <= d;
            }
        }
#endif
    }
}
