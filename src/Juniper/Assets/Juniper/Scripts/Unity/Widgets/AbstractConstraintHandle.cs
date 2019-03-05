using UnityEngine;

namespace Juniper.Unity.Widgets
{
    /// <summary>
    /// An abstract class that manages reading the movement of a control handle and constraining that
    /// movement along an axis. The implementing child class will then interpret that movement into a
    /// transformation like Translation or Rotation.
    /// </summary>
    public abstract class AbstractConstraintHandle : MonoBehaviour
    {
        /// <summary>
        /// The object that is being transformed.
        /// </summary>
        public Transform target;

        /// <summary>
        /// The axis on which the transform is performing.
        /// </summary>
        public CartesianAxis axis = CartesianAxis.Y;

        /// <summary>
        /// The proportion of the handle movement that is turned into target translation.
        /// </summary>
        [Range(0, 1)]
        public float speed = 1;

        /// <summary>
        /// Sets the target for all handles in a parent element.
        /// </summary>
        /// <returns>The setup.</returns>
        /// <param name="handleParent">Handle parent.</param>
        /// <param name="target">Target.</param>
        /// <param name="disableHandles">If set to <c>true</c> disable handles.</param>
        public static AbstractConstraintHandle[] Setup(Transform handleParent, Transform target, bool disableHandles)
        {
            var handles = handleParent.gameObject.GetComponentsInChildren<AbstractConstraintHandle>();
            foreach (var handle in handles)
            {
                handle.target = target;
                if (disableHandles)
                {
                    handle.Deactivate();
                }
            }
            return handles;
        }

        /// <summary>
        /// Finds the <see cref="Draggable"/> handle that will be used to perform the transformation operation.
        /// </summary>
        public void Awake()
        {
            handle = GetComponent<Draggable>();
        }

        /// <summary>
        /// Reads the change in the location of the handle, constrains it to a specific axis, and
        /// asks the child implementing class to apply the change to the target object.
        /// </summary>
        public void Update()
        {
            var haveTarget = target != null;

            if (haveTarget && !hadTarget)
            {
                lastPosition = startPosition = transform.localPosition;
            }

            hadTarget = haveTarget;

            if (haveTarget)
            {
                if (handle.Dragged)
                {
                    var delta = GetChange();
                    var v = ConstrainChange(delta);
                    ApplyChange(v);
                }
                else
                {
                    transform.localPosition = lastPosition = startPosition;
                }
            }
        }

        /// <summary>
        /// The position at which the handle started life.
        /// </summary>
        protected Vector3 startPosition;

        /// <summary>
        /// The position of the handle at the end of the last frame, for detecting the movement delta
        /// of the handle between frames.
        /// </summary>
        protected Vector3 lastPosition;

        /// <summary>
        /// Deletes component values from the movement delta that do not support moving in the
        /// object's configured <see cref="axis"/>.
        /// </summary>
        /// <returns>The change.</returns>
        /// <param name="v">V.</param>
        protected Vector3 ConstrainChange(Vector3 v)
        {
            if (axis != CartesianAxis.X)
            {
                v.x = 0;
            }

            if (axis != CartesianAxis.Y)
            {
                v.y = 0;
            }

            if (axis != CartesianAxis.Z)
            {
                v.z = 0;
            }

            return v;
        }

        /// <summary>
        /// Implementing classes override this method to interpret the movement delta into a
        /// constrainable expression.
        /// </summary>
        /// <returns>The change.</returns>
        protected abstract Vector3 GetChange();

        /// <summary>
        /// Implementing classes receive the constrained expression and interpret it into a
        /// transformation on the target.
        /// </summary>
        /// <param name="v">V.</param>
        protected abstract void ApplyChange(Vector3 v);

        /// <summary>
        /// Set to true if <see cref="target"/> was not null at the end of the last frame. Used to
        /// recognize when the target has been newly added to the constraint.
        /// </summary>
        private bool hadTarget;

        /// <summary>
        /// The handle that is doing the transforming.
        /// </summary>
        private Draggable handle;
    }
}
