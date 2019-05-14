using Juniper.Display;

using UnityEngine;

namespace Juniper.Widgets
{
    /// <summary>
    /// Holds an object away from the user on the opposite side of a target object. This is useful
    /// for making UIs that appear above and behind an object no matter where the user is standing.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Standoff : MonoBehaviour
    {
        /// <summary>
        /// The object that shall be kept between the user and the UI.
        /// </summary>
        public Transform target;

        /// <summary>
        /// The distance, in meters, the object will be held away from the target.
        /// </summary>
        public float distance = 1;

        /// <summary>
        /// The height, in meters, above the target at which the object will be held.
        /// </summary>
        public float height = 1;

        /// <summary>
        /// Moves the object into position above and behind the target, away from the user.
        /// </summary>
        public void Update()
        {
            if (camT == null)
            {
                camT = DisplayManager.MainCamera.transform;
            }

            if (target != null)
            {
                var delta = target.position - camT.position;
                delta.y = 0;
                delta.Normalize();
                delta *= distance;
                delta.y = height;

                transform.position = target.position + delta;
                delta.y = 0;
                transform.rotation = Quaternion.LookRotation(delta);
            }
        }

        /// <summary>
        /// Immediately updates the location of the object when it is enabled so that it doesn't look
        /// like it is snapping into place on the second frame.
        /// </summary>
        public void OnEnable()
        {
            Update();
        }

        /// <summary>
        /// The main camera.
        /// </summary>
        private Transform camT;
    }
}
