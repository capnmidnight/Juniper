using System;

using Juniper.Anchoring;
using Juniper.Display;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    /// <summary>
    /// The Valued component adds a float field and range of valid values for that field to a
    /// Touchable object.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public abstract class AbstractValued : AbstractTouchable,
        IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler,
        IEndDragHandler, IDropHandler, IValuedControl<float>
    {
        /// <summary>
        /// The physical distance the control has moved.
        /// </summary>
        protected float dragDistance;

        /// <summary>
        /// The graphic to update with the current value of the control.
        /// </summary>
        public TextMesh Display;

        /// <summary>
        /// The number of significant figures to which to format the number.
        /// </summary>
        [Range(0, 10)]
        public int SigFigs = 3;

        /// <summary>
        /// The output value will be rescaled from the domain [0, 1] to the range [ <see
        /// cref="minValue"/>, <see cref="maxValue"/>]
        /// </summary>
        [Tooltip("The output value will be rescaled from the domain [0, 1] to the range [minValue, maxValue]")]
        public float minValue;

        /// <summary>
        /// The output value will be rescaled from the domain [0, 1] to the range [ <see
        /// cref="minValue"/>, <see cref="maxValue"/>]
        /// </summary>
        [Tooltip("The output value will be rescaled from the domain [0, 1] to the range [minValue, maxValue]")]
        public float maxValue;

        /// <summary>
        /// The output value will be rescaled from the domain [0, 1] to the range [ <see
        /// cref="minValue"/>, <see cref="maxValue"/>]
        /// </summary>
        [Tooltip("The output value will be rescaled from the domain [0, 1] to the range [minValue, maxValue]")]
        public float _value;

        /// <summary>
        /// The output value will be rescaled from the domain [0, 1] to the range [ <see
        /// cref="minValue"/>, <see cref="maxValue"/>]
        /// </summary>
        /// <remarks>
        /// To satisfy the IValuedControl interface, value needs to be a property. But to work with
        /// the Unity Editor, it needs to be a field. So we have to manually create our own property backing.
        /// </remarks>
        public float value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// The difference between <see cref="maxValue"/> and <see cref="minValue"/>
        /// </summary>
        /// <value>The delta.</value>
        public float Delta
        {
            get
            {
                return maxValue - minValue;
            }
        }

        /// <summary>
        /// Is the control currently being dragged?
        /// </summary>
        public bool Dragged;

        /// <summary>
        /// Was the control being dragged on the last frame?
        /// </summary>
        private bool wasDragged;

        /// <summary>
        /// Did the <see cref="Dragged"/> flag change since the last frame?
        /// </summary>
        private bool DraggedChanged
        {
            get
            {
                return Dragged != wasDragged;
            }
        }

        /// <summary>
        /// Returns true if the control was <see cref="Pressed"/> during this frame (but not the last).
        /// </summary>
        public bool DraggedDown
        {
            get
            {
                return DraggedChanged && Dragged;
            }
        }

        /// <summary>
        /// Returns true if the control was <see cref="Dragged"/> in the previous frame, but not the
        /// current one.
        /// </summary>
        public bool DraggedUp
        {
            get
            {
                return DraggedChanged && !Dragged;
            }
        }

        /// <summary>
        /// The point in 3D space where the user's selection occurred. When transitioning from no
        /// selection to having a selection, the manipulationPoint will be set as the raycast point
        /// where the user made the selection. In subsequent updates, the manipulationPoint will move
        /// from that starting point in proportion to the user's movements.
        /// </summary>
        protected Vector3 interactionPoint;

        /// <summary>
        /// The delta from the manipulationPoint to the origin of the object. This is useful for
        /// making sure the user can grab an object anywhere on the object and not have the object
        /// warp to put the object's origin in their grabber.
        /// </summary>
        protected Vector3 interactionOffset;

        /// <summary>
        /// The <see cref="value"/> during the last frame, to check to see if the value changed in
        /// this frame.
        /// </summary>
        private float lastValue;

        /// <summary>
        /// The rate at which the object is being moved through space while it is being held by the user.
        /// </summary>
        protected Vector3 velocity;

        /// <summary>
        /// The rate at which the object is being rotated while it is being held by the user.
        /// </summary>
        protected Vector3 angVel;

        /// <summary>
        /// The main camera, which is used to figure out changes in view perspective if the object is
        /// supposed to rotate with the user.
        /// </summary>
        protected Transform camT;

        /// <summary>
        /// The offset between the center of the object and where the pointer first grabbed the
        /// object, used to figure out what the desired movement vector was without snapping the
        /// object's center to the pointer location.
        /// </summary>
        protected Vector3 grabOffset;

        /// <summary>
        /// The last value at which a tick sound was played.
        /// </summary>
        private float lastTickValue;

        /// <summary>
        /// Returns true if <see cref="value"/> has changed more than 0.0001.
        /// </summary>
        /// <value><c>true</c> if value changed; otherwise, <c>false</c>.</value>
        protected bool ValueChanged
        {
            get
            {
                return !Mathf.Approximately(value, lastValue);
            }
        }

        /// <summary>
        /// An event for detecting when the Draggable has changed.
        /// </summary>
        public event EventHandler<float> ValueChange;

        /// <summary>
        /// This `internalValue` field should be calculated to the range [0, ConstraintRange] by the
        /// implementing class in the `UpdateValue()` method.
        /// </summary>
        protected float internalValue;

        /// <summary>
        /// The range of the input values, for reseting to a [0, 1] range so that the Valued class
        /// can rescale to the user's desired range.
        /// </summary>
        protected virtual float ConstraintRange
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Set the internal value of the control, and update its display.
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetValue(float value)
        {
            this.value = value;
            Update();
        }

        /// <summary>
        /// After all user input has been resolved, update the saved value of the control, for
        /// changing its rendered representation.
        /// </summary>
        protected override void LateUpdate()
        {
            if (camT == null)
            {
                camT = DisplayManager.MainCamera.transform;
            }

            if (Display == null)
            {
                Display = this.Query<TextMesh>("Display");
            }

            var delta = Mathf.Abs(internalValue - lastTickValue);
            if (delta > 1)
            {
                lastTickValue = internalValue;
            }

            base.LateUpdate();

            if (ValueChanged)
            {
                ValueChange?.Invoke(this, value);
            }

            wasDragged = Dragged;
            lastValue = value;
            UpdateInternalValue();
        }

        /// <summary>
        /// Update the saved value of the control, to change its rendered representation.
        /// </summary>
        protected void UpdateInternalValue()
        {
            value = internalValue / ConstraintRange;
            if (minValue < maxValue)
            {
                value = minValue + (value * Delta);
            }

            if (Display != null)
            {
                Display.text = value.SigFig(SigFigs);
            }
        }

        /// <summary>
        /// Useful for updating the internal state of the control when setting the value externally.
        /// This method undoes the scaling that happens at the end of `OnUpdate()`.
        /// </summary>
        protected void ResetInternalValue()
        {
            if (minValue < maxValue)
            {
                var delta = maxValue - minValue;
                internalValue = ConstraintRange * (value - minValue) / delta;
            }
        }

        /// <summary>
        /// When the user first presses the button down on a draggable element, we initialize
        /// tracking when the drag threshold has been crossed.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
        }

        /// <summary>
        /// React to the start of the dragging process on this control and updates the control value
        /// based on how far it has been dragged.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == clickButton
                && !IsDisabled
                && !Dragged)
            {
                Dragged = true;
                GetPosition(eventData);
                dragDistance = 0;
                lastTickValue = internalValue;
                grabOffset = pointerPosition - transform.position;
                var anchor = GetComponent<Anchored>();
                anchor?.WeighAnchor();
            }
        }

        /// <summary>
        /// Updates the control value based on how far it has been dragged.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == clickButton
                && !IsDisabled
                && Dragged)
            {
                GetPosition(eventData);
                interactionOffset = pointerPosition - interactionPoint - grabOffset;
                var delta = pointerPosition - lastPointerPosition;
                dragDistance += delta.magnitude;
            }
        }

        /// <summary>
        /// React to the primary selection button being released on the control while in the dragging state.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnDrop(PointerEventData eventData)
        {
        }

        /// <summary>
        /// Updates the control value based on how far it has been dragged.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == clickButton
                && !IsDisabled
                && Dragged)
            {
                GetPosition(eventData);
                interactionPoint = Vector3.zero;
                interactionOffset = Vector3.zero;
                grabOffset = Vector3.zero;
                GetComponent<Anchored>()?.DropAnchor();
            }
        }
    }
}
