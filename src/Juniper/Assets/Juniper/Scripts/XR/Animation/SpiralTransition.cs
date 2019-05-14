using Juniper.Display;

using UnityEngine;

namespace Juniper.Animation
{
    /// <summary>
    /// Take an object in world-space and spiral it in to camera-locked space (faux-screen-space)
    /// </summary>
    public class SpiralTransition : AbstractTransitionController
    {
        /// <summary>
        /// The local rotation of the element when sitting in its original parent.
        /// </summary>
        public Vector3 startOrientation = 90 * Vector3.right;

        /// <summary>
        /// The local rotation of the element when it is parented tracking the camera.
        /// </summary>
        public Vector3 endOrientation = 90 * Vector3.back;

        /// <summary>
        /// Set to true if the transition is being used to make a video appear as if it is
        /// full-screen. The <see cref="distance"/> field will automatically be calculated to make
        /// the extents of the video fill the view port.
        /// </summary>
        public bool fullScreen = true;

        /// <summary>
        /// The distance from the camera at which the element should stop when the transition has
        /// finished entering. The value entered in the Editor for this field is ignored if <see
        /// cref="fullScreen"/> is set to true.
        /// </summary>
        public float distance = 0.2f;

        /// <summary>
        /// The amount of time, in seconds, that the transition should take.
        /// </summary>
        public float length = 1;

        /// <summary>
        /// The amount of time, in seconds, that the transition should take.
        /// </summary>
        public override float TransitionLength
        {
            get
            {
                return length;
            }
        }

        /// <summary>
        /// Finds the transform of the main camera.
        /// </summary>
        public void Awake()
        {
            camT = DisplayManager.MainCamera.transform;
        }

        /// <summary>
        /// Continuously update the location, rotation, and scale of the element.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (fullScreen)
            {
                var mid = DisplayManager.MainCamera.ViewportToWorldPoint(Vector3.forward);
                var top = DisplayManager.MainCamera.ViewportToWorldPoint(new Vector3(0, 1, 1));
                var delta = top - mid;
                mid *= 0.5f / delta.magnitude;
                distance = mid.magnitude * 0.975f;
            }

            var rotationStart = startParent.rotation * Quaternion.Euler(startOrientation);
            var rotationEnd = camT.rotation * Quaternion.Euler(endOrientation);
            var endPosition = camT.position + distance * camT.forward;

            transform.rotation = Quaternion.Slerp(rotationStart, rotationEnd, tweenValue);
            transform.position = Vector3.Lerp(startPosition, endPosition, tweenValue);
            transform.localScale = Vector3.Lerp(startScale, endScale, tweenValue);
        }

        /// <summary>
        /// Records the transition value so the <see cref="Update"/> function can use it.
        /// </summary>
        /// <param name="value">Value.</param>
        protected override void RenderValue(float value)
        {
            tweenValue = value;
        }

        /// <summary>
        /// Records the starting values so the enter transition can LERP over the values correctly.
        /// </summary>
        protected override void OnEntering()
        {
            startParent = transform.parent;
            startPosition = transform.position;
            endScale = transform.localScale;
            transform.parent = null;
            startScale = transform.localScale;
            base.OnEntering();
        }

        /// <summary>
        /// Records the exiting values to make the update LERP undo the changes to the element.
        /// </summary>
        protected override void OnExiting()
        {
            startScale = endScale;
            transform.parent = startParent;
            endScale = transform.localScale;
            base.OnExiting();
        }

        /// <summary>
        /// The object under which the element originally started.
        /// </summary>
        private Transform startParent;

        /// <summary>
        /// The position at which the element started.
        /// </summary>
        private Vector3 startPosition;

        /// <summary>
        /// The scale of the element at the start of the enter transition, or the end of the exit transition.
        /// </summary>
        private Vector3 startScale;

        /// <summary>
        /// The scale of the element at the end of the enter transition, or the start of the exit transition.
        /// </summary>
        private Vector3 endScale;

        /// <summary>
        /// The camera, cached for faster access.
        /// </summary>
        private Transform camT;

        /// <summary>
        /// The current transition state value, cached so the location and rotation can change with
        /// the camera after the transition has finished.
        /// </summary>
        private float tweenValue;
    }
}
