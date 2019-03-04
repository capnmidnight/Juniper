using Juniper.Animation;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    /// <summary>
    /// A control that is a container of other controls that are hidden until the control is clicked.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Openable : Clickable
    {
        /// <summary>
        /// The scene sub-graph to show when this control is opened.
        /// </summary>
        public Transform payload;

        /// <summary>
        /// Is the control in its open state?
        /// </summary>
        public bool Open;

        /// <summary>
        /// Did the <see cref="Open"/> state change since the last frame?
        /// </summary>
        public bool OpenChanged
        {
            get
            {
                return Open != wasOpen;
            }
        }

        /// <summary>
        /// Did the control <see cref="Open"/> on this frame?
        /// </summary>
        /// <value><c>true</c> if opened; otherwise, <c>false</c>.</value>
        public bool Opened
        {
            get
            {
                return OpenChanged && Open;
            }
        }

        /// <summary>
        /// Did the value of <see cref="Open"/> become false on this frame?
        /// </summary>
        /// <value><c>true</c> if closed; otherwise, <c>false</c>.</value>
        public bool Closed
        {
            get
            {
                return OpenChanged && !Open;
            }
        }

        /// <summary>
        /// If the control is not disabled, sets the open state.
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public void SetOpen(bool value)
        {
            if (!IsDisabled && Open != value)
            {
                Open = value;
                if (stater == null)
                {
                    payload.SetActive(Open);
                }
                else if (value)
                {
                    payload.Activate();
                    stater.Enter();
                }
                else
                {
                    stater.Exit();
                    payload.Deactivate();
                }

                ShowState(value ? "Opened" : "Closed");
            }
        }

        /// <summary>
        /// If the control is closed, open it. If it's open, close it.
        /// </summary>
        public void ToggleOpen()
        {
            SetOpen(!Open);
        }

        /// <summary>
        /// If the control is not disabled, toggles the open state.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            ToggleOpen();
        }

        /// <summary>
        /// Finds the <see cref="payload"/> and checks to see if it's already open.
        /// </summary>
        protected override void Awake()
        {
            payload = transform.Find("Payload");

            stater = payload.GetComponent<AbstractTransitionController>();

            base.Awake();
        }

        /// <summary>
        /// Make sure the state hasn't changed outside of our control.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (Open == wasOpen)
            {
                Open = payload.IsActivated();
            }
            else
            {
                // replay the change
                Open = !Open;
                wasOpen = !wasOpen;
                SetOpen(wasOpen);
            }
        }

        /// <summary>
        /// Save whether or not the control was open.
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();
            wasOpen = Open;
        }

        /// <summary>
        /// If the payload has a TransitionController on it, then Openable will use it as part of the
        /// open/close process.
        /// </summary>
        private AbstractTransitionController stater;

        /// <summary>
        /// The <see cref="Open"/> state of the container from the last frame. On startup, there is
        /// no "last frame", so the value initializes to null for three-value logic.
        /// </summary>
        private bool wasOpen;
    }
}
