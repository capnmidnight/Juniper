using Juniper.Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    /// <summary>
    /// A control that is rotatable in a single axis at a time.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Rotatable : AbstractValued
    {
        /// <summary>
        /// The axis on which to rotate the object.
        /// </summary>
        public CartesianAxis rotationAxis;

        /// <summary>
        /// How much to scale the global interpolation factor to get a local one. This makes it
        /// easier to enforce whether or not interpolation factors are in harmonic resonance with
        /// each other, if that is/is not desired.
        /// </summary>
        [Range(0, 1)]
        public float InterpolationScale = 0.9f;

        /// <summary>
        /// When the gameObject gets enabled, restore the <see cref="endValue"/> of the control.
        /// </summary>
        public void OnEnable()
        {
            SetValue(endValue);
        }

        /// <summary>
        /// When the gameObject gets disabled, save the current value of the control to <see
        /// cref="endValue"/> and reset the control's state to <see cref="startValue"/>.
        /// </summary>
        public void OnDisable()
        {
            endValue = value;
            SetValue(startValue);
        }

        /// <summary>
        /// Initialize the rotation at the beginning of the drag operation.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (!IsDisabled)
            {
                lastInteractionPoint = interactionPoint;
            }
        }

        /// <summary>
        /// Update the rotation of the object based on a drag gesture.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (!IsDisabled)
            {
                // The rotation will be calculated based on the difference between the vector from
                // the origin of the object to A) where we started from and B) where end ended at.
                var newInteractionPoint = interactionPoint + interactionOffset + grabOffset;
                var toStart = lastInteractionPoint - transform.position;
                var toEnd = newInteractionPoint - transform.position;

                lastInteractionPoint = newInteractionPoint;

                // For now, arbitrary axis of rotation is not supported. Only the Three cartesian
                // axes are. But they will work "correctly", despite any parent object rotations,
                // unlike Unity's axis of rotation constraint.
                if (rotationAxis == CartesianAxis.X)
                {
                    toStart.x = toEnd.x = 0;
                }
                else if (rotationAxis == CartesianAxis.Y)
                {
                    toStart.y = toEnd.y = 0;
                }
                else if (rotationAxis == CartesianAxis.Z)
                {
                    toStart.z = toEnd.z = 0;
                }

                amount = Quaternion.FromToRotation(toStart, toEnd);
            }
        }

        /// <summary>
        /// To satisfy the needs of the parent Valued class, convert the angle of rotation to number
        /// of whole rotations.
        /// </summary>
        protected override float ConstraintRange
        {
            get
            {
                return 360f;
            }
        }

        /// <summary>
        /// Initialize the control, and set the current value as <see cref="startValue"/>.
        /// </summary>
        public void Awake()
        {
            if (minValue >= maxValue)
            {
                minValue = 0;
                maxValue = 360;
            }

            startValue = value;
        }

        /// <summary>
        /// If the represented value has changed since the last frame, the rotation of the object of
        /// the object will be changed. Otherwise, a new value will be calculated from the rotation
        /// of the object and how far it has been rotated away from its starting point.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (angleCorrector == null)
            {
                angleCorrector = new Angle(value);
            }

            if (!IsDisabled)
            {
                if (ValueChanged)
                {
                    // If the value was changed from an external script.
                    ResetInternalValue();
                    angleCorrector = (Angle)internalValue;
                    amount = Quaternion.identity;
                    var eul = Vector3.zero;
                    if (rotationAxis == CartesianAxis.X)
                    {
                        eul.x = internalValue;
                    }
                    else if (rotationAxis == CartesianAxis.Y)
                    {
                        eul.y = internalValue;
                    }
                    else if (rotationAxis == CartesianAxis.Z)
                    {
                        eul.z = internalValue;
                    }

                    transform.localRotation = Quaternion.Euler(-eul);
                }
                else
                {
                    transform.rotation *= amount;

                    var euler = -transform.localEulerAngles;
                    if (rotationAxis == CartesianAxis.X)
                    {
                        internalValue = euler.x;
                    }
                    else if (rotationAxis == CartesianAxis.Y)
                    {
                        internalValue = euler.y;
                    }
                    else if (rotationAxis == CartesianAxis.Z)
                    {
                        internalValue = euler.z;
                    }

                    angleCorrector ^= internalValue;
                    internalValue = angleCorrector;
                    UpdateInternalValue();
                    amount = Quaternion.Slerp(Quaternion.identity, amount, InterpolationScale);
                }
            }
        }

        /// <summary>
        /// The cursor interaction point from the last frame, used to calcuate a motion delta with
        /// the current frame, which in turn is used to calculate the amount of rotation.
        /// </summary>
        private Vector3 lastInteractionPoint;

        /// <summary>
        /// The amount to rotate on this frame. This value is Slerped with the previous frame's value
        /// to make for smoother rotations.
        /// </summary>
        private Quaternion amount;

        /// <summary>
        /// The angle corrector makes sure the rotatable object never attempts to Lerp 359 degrees to
        /// 1 degrees, or vice versa. The cross-360 degree barrier is smoothed over to make for
        /// smaller, more natural changes.
        /// </summary>
        private Angle angleCorrector;

        /// <summary>
        /// The value the control had when the application was first started.
        /// </summary>
        private float startValue;

        /// <summary>
        /// The value the control had just before it was disabled.
        /// </summary>
        private float endValue;
    }
}
