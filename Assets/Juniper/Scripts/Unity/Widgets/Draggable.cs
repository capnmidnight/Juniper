using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    /// <summary>
    /// An object that can be picked up and put down in a new location.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Draggable : AbstractValued
    {
        /// <summary>
        /// The name of the gameObject to create when creating the motion contstraint's starting point.
        /// </summary>
        private const string MotionConstraintStartName = "MotionConstraintStart";

        /// <summary>
        /// The name of the gameObject to create when creating the motion constraint's ending point.
        /// </summary>
        private const string MotionConstraintEndName = "MotionConstraintEnd";

        /// <summary>
        /// If constrainMotion is false, the motion constraint vector is an infinitely long ray. If
        /// it's true, then the motion constraint vector is a line segment that the object cannot exceed.
        /// </summary>
        public bool constrainMotion;

        /// <summary>
        /// The beginning of the motion constraint track.
        /// </summary>
        private Transform motionConstraintStart;

        /// <summary>
        /// The end of the motion constraint track.
        /// </summary>
        private Transform motionConstraintEnd;

        /// <summary>
        /// When set to true, the draggable object will be reoriented to keep the same relative
        /// orientation to the user who is doing the dragging as when the dragging started.
        /// </summary>
        public bool rotateWhileDragging = false;

        /// <summary>
        /// The position of the camera in the last frame, so we can calculate the orientation offset
        /// if <see cref="rotateWhileDragging"/> is set to true.
        /// </summary>
        private Vector3 lastCamPosition;

        /// <summary>
        /// If <see cref="motionConstraintStart"/> is not null, returns its position. Otherwise,
        /// returns the Zero Vector.
        /// </summary>
        private Vector3 TrackStart =>
            motionConstraintStart ? motionConstraintStart.position : Vector3.zero;

        /// <summary>
        /// If <see cref="motionConstraintEnd"/> is not null, returns its position. Otherwise,
        /// returns the Zero Vector.
        /// </summary>
        private Vector3 TrackEnd =>
            motionConstraintEnd ? motionConstraintEnd.position : Vector3.zero;

#if UNITY_EDITOR

        /// <summary>
        /// If <see cref="constrainMotion"/> is set to true, draws a line between the <see
        /// cref="TrackStart"/> and <see cref="TrackEnd"/> in the Unity Editor.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (constrainMotion)
            {
                Gizmos.DrawLine(TrackStart, TrackEnd);
            }
        }

#endif

        /// <summary>
        /// Find the motion constraint endpoint, or create it if it doesn't exist
        /// </summary>
        /// <param name="t">the place to store the constraint</param>
        /// <param name="constraintName">the name of the constraint as a sibling of this element</param>
        /// <param name="position">the default location of the constraint, if it needs to be created</param>
        private void ResolveConstraint(ref Transform t, string constraintName, Vector3 position)
        {
            if (t == null)
            {
                t = transform.parent.Find(constraintName);
                if (t == null)
                {
                    t = transform.parent.EnsureTransform(constraintName);
                    t.localPosition = position;
                }
            }
        }

        /// <summary>
        /// Destroy a motion constraint if it exists and we no longer need it.
        /// </summary>
        /// <param name="t">the place the motion constraint is usually stored</param>
        /// <param name="constraintName">the name of the constraint as a sibling of this element</param>
        private void DestroyConstraint(ref Transform t, string constraintName)
        {
            if (t == null && transform.parent != null)
            {
                t = transform.parent.Find(constraintName);
            }

            if (t != null)
            {
                t.gameObject.Destroy();
            }
        }

        /// <summary>
        /// Validate the motion constraint, and if one exists, constraint the motion of the draggable
        /// object. Additioinally, recompute the internal value of the object.
        /// </summary>
        public void OnValidate()
        {
            // if the range is wrong, reset it to something more meaningful
            if (minValue >= maxValue)
            {
                minValue = 0;
                maxValue = 1;
            }

            // make sure we have the right set of objects in the scene.
            if (constrainMotion)
            {
                ResolveConstraint(ref motionConstraintStart, MotionConstraintStartName, Vector3.zero);
                ResolveConstraint(ref motionConstraintEnd, MotionConstraintEndName, Vector3.right);
            }
            else if (!constrainMotion)
            {
                DestroyConstraint(ref motionConstraintStart, MotionConstraintStartName);
                DestroyConstraint(ref motionConstraintEnd, MotionConstraintEndName);
            }
        }

        /// <summary>
        /// Save the changes to the dragged object.
        /// </summary>
        protected override void LateUpdate()
        {
            if (!Dragged && ValueChanged)
            {
                // if the user didn't drag the object, but some other script has changed the value,
                // we need to update the position of the slider pip.
                ResetInternalValue();
                if (constrainMotion)
                {
                    transform.position = Vector3.Lerp(TrackStart, TrackEnd, internalValue);
                }
            }

            base.LateUpdate();
        }

        /// <summary>
        /// Updates the position of the control When the control receives the OnDrag event from the EventSystem.
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (!IsDisabled)
            {
                var destination = interactionPoint + interactionOffset;

                if (constrainMotion)
                {
                    // project the user's movement vector...
                    var delta = destination - TrackStart;
                    // onto the constraint vector...
                    var constraint = TrackEnd - TrackStart;

                    // the projection requires a unit vector
                    var length = constraint.magnitude;
                    constraint /= length;

                    // the dot product of a vector with a unity vector is defined as the amount of
                    // the input vector that is projected on the reference vector.
                    internalValue = Vector3.Dot(delta, constraint) / length;

                    // we don't want to exceed the range of the constraint.
                    internalValue = Mathf.Clamp(internalValue, 0, 1);
                    destination = Vector3.Lerp(TrackStart, TrackEnd, internalValue);
                }
                else
                {
                    internalValue = dragDistance;
                }

                if (rotateWhileDragging)
                {
                    var r = Quaternion.FromToRotation(transform.position - lastCamPosition, destination - camT.position);

                    lastCamPosition = camT.position;

                    var finalRotation = r * transform.rotation;
                    var dEul = finalRotation.eulerAngles.DeltaAngle(transform.eulerAngles);
                    angVel = (dEul.x * transform.right
                        + dEul.y * transform.up
                        + dEul.z * transform.forward) / Time.deltaTime;
                    transform.rotation = finalRotation;
                }

                velocity = (destination - transform.position) / Time.deltaTime;
                transform.position = destination;
            }
        }
    }
}
