using System.Collections.Generic;

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

        public static Juniper.XR.Pose ToJuniperPose(this Transform t)
        {
            return new Juniper.XR.Pose(t.position.ToNETFXVector(), t.rotation.ToNETFXQuaternion());
        }

        public static System.Numerics.Vector3 ToNETFXVector(this Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.x, vector.y, vector.z);
        }

        public static System.Numerics.Quaternion ToNETFXQuaternion(this Quaternion quat)
        {
            return new System.Numerics.Quaternion(quat.x, quat.y, quat.z, quat.w);
        }

        public static Vector3 ToUnityVector(this System.Numerics.Vector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static Quaternion ToUnityQuaternion(this System.Numerics.Quaternion quat)
        {
            return new Quaternion(quat.X, quat.Y, quat.Z, quat.W);
        }
    }
}
