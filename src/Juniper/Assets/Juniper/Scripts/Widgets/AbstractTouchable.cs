using System.Linq;

using Juniper.Animation;

using UnityEngine;
using UnityEngine.EventSystems;

using InputButton = UnityEngine.EventSystems.PointerEventData.InputButton;

namespace Juniper.Widgets
{
    /// <summary>
    /// The base class for all interactive, AR-compatible, GUI controls /// This class is abstract to
    /// avoid it being used directly as a component. You must subclass this component and create a
    /// new component out of it.
    /// </summary>
    [DisallowMultipleComponent]
#if UNITY_MODULES_PHYSICS
    [RequireComponent(typeof(Collider))]
#endif
    [ExecuteInEditMode]
    public abstract class AbstractTouchable :
        MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// The names of the animation states that Touchable expects.
        /// </summary>
        public static string[] PART_NAMES =
        {
            "Normal",
            "Hovered",
            "Pressed",
            "Opened",
            "Disabled"
        };

        /// <summary>
        /// The button we are expecting to use to interact with this object.
        /// </summary>
        public InputButton clickButton;

        /// <summary>
        /// An object responsible for changing the visible representation of the control when the
        /// control's state changes.
        /// </summary>
        [HideInNormalInspector]
        [SerializeField]
        private AbstractAnimator animator;

        /// <summary>
        /// Should the control run in a "disabled" state? It is still actively executing code, it's
        /// just preventing the main data-function of the control from occurring. There is still a
        /// need to show a different UI and react to user input to indicate the control is disabled.
        /// </summary>
        public bool disabled;

        /// <summary>
        /// The value of <see cref="IsDisabled"/> in the last frame.
        /// </summary>
        private bool? wasDisabled;

        /// <summary>
        /// Returns true when the value of <see cref="IsDisabled"/> changes from one frame to the next.
        /// </summary>
        /// <value><c>true</c> if disabled changed; otherwise, <c>false</c>.</value>
        private bool DisabledChanged
        {
            get
            {
                return IsDisabled != wasDisabled;
            }
        }

        /// <summary>
        /// Returns true when the value of <see cref="IsDisabled"/> goes from false to true.
        /// </summary>
        /// <value><c>true</c> if disabled down; otherwise, <c>false</c>.</value>
        private bool DisabledDown
        {
            get
            {
                return DisabledChanged && IsDisabled;
            }
        }

        /// <summary>
        /// Returns true when the value of <see cref="IsDisabled"/> goes from true to false.
        /// </summary>
        /// <value><c>true</c> if disabled up; otherwise, <c>false</c>.</value>
        private bool DisabledUp
        {
            get
            {
                return DisabledChanged && !IsDisabled;
            }
        }

        /// <summary>
        /// Returns true if the gameObject is not active, if the MonoBehaviour is not enabled, or if
        /// the Statable state has been set to Disabled.
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return !isActiveAndEnabled || disabled;
            }

            set
            {
                disabled = value;
            }
        }

        /// <summary>
        /// Set whether or not the control should operate in the "Disabled" state. See <see
        /// cref="IsDisabled"/> for more information.
        /// </summary>
        /// <param name="value"></param>
        public void SetDisabled(bool value)
        {
            IsDisabled = value;
        }

        /// <summary>
        /// Toggles the boolean value of the `disabled` field.
        /// </summary>
        public void ToggleEnabled()
        {
            IsDisabled = !IsDisabled;
        }

        /// <summary>
        /// The place near the control that the pointer was pointing.
        /// </summary>
        public Vector3 pointerPosition;

        /// <summary>
        /// The <see cref="AbstractTouchable.pointerPosition"/> from the previous frame, used for
        /// calculating how far the pointer has moved since the last frame.
        /// </summary>
        protected Vector3 lastPointerPosition;

        /// <summary>
        /// Returns true when <see cref="pointerPosition"/> has changed since the last frame.
        /// </summary>
        /// <value><c>true</c> if pointer position changed; otherwise, <c>false</c>.</value>
        protected bool PointerPositionChanged
        {
            get
            {
                return pointerPosition != lastPointerPosition;
            }
        }

        private void Awake()
        {
            animator = AbstractAnimator.GetAnimator(gameObject, PART_NAMES);
        }

        /// <summary>
        /// Update the physics state of the control, and enable/disable Disabled view on <see cref="DisabledChanged"/>.
        /// </summary>
        public virtual void Update()
        {
            if (DisabledDown)
            {
                ShowState(PART_NAMES.Last());
            }
            else if (DisabledUp)
            {
                ShowState(PART_NAMES[0]);
            }
        }

        /// <summary>
        /// Saves the state of the flags from the current frame to have them for the next frame.
        /// </summary>
        protected virtual void LateUpdate()
        {
            wasDisabled = IsDisabled;
            lastPointerPosition = pointerPosition;
        }

        private string NormalizePart(string enabledPart)
        {
            // If the desired state doesn't have a graphics object to display, then just display the
            // default state.
            if (enabledPart == null
                || animator != null && !animator.HasState(enabledPart))
            {
                enabledPart = PART_NAMES[0];
            }

            return enabledPart;
        }

        /// <summary>
        /// Trigger the animation or swap the gameObjects to show the current control UI state.
        /// </summary>
        /// <returns>The state.</returns>
        /// <param name="enabledPart">Enabled part.</param>
        protected string ShowState(string enabledPart)
        {
            enabledPart = NormalizePart(enabledPart);
            animator?.Play(enabledPart);
            return enabledPart;
        }

        /// <summary>
        /// Saves the worldSpace position of any pointer events that were fired on this object. The
        /// worldSpace position is then used by Draggable objects to figure out where to move them.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        protected void GetPosition(PointerEventData eventData)
        {
            pointerPosition = eventData.pointerCurrentRaycast.worldPosition;
            eventData.Use();
        }

        /// <summary>
        /// React to the pointer hovering over the control.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.button == clickButton && !IsDisabled)
            {
                GetPosition(eventData);
                ShowState("Hovered");
            }
        }

        /// <summary>
        /// React to the pointer leaving the control and no longer hovering over it.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.button == clickButton && !IsDisabled)
            {
                GetPosition(eventData);
                ShowState("Normal");
            }
        }

        /// <summary>
        /// React to the primary button being pressed on the control.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == clickButton)
            {
                if (IsDisabled)
                {
                    ShowState("Disabled");
                }
                else
                {
                    GetPosition(eventData);
                    ShowState("Pressed");
                }
            }
        }

        /// <summary>
        /// React to the primary button being released from the control.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == clickButton && !IsDisabled)
            {
                GetPosition(eventData);
                ShowState("Hovered");
            }
        }
    }
}
