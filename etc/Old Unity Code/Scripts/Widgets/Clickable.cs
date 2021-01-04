using System;

using Juniper.Events;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    /// <summary>
    /// An object that can be Touched on a screen or Clicked with a mouse and fires an event in response.
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Clickable :
        AbstractTouchable,
        IPointerClickHandler,
        ILongPressHandler
    {
        /// <summary>
        /// The event to call when the button is enabled and clicked. You should only use this
        /// version in the Unity Editor. If you are programmatically attaching event listeners, you
        /// should preferr <see cref="Click"/>.
        /// </summary>
        public UnityEvent onClick;

        public UnityEvent onLongPress;

        /// <summary>
        /// The event to call when the button is enabled and clicked. You should only use this
        /// verison if you are programmatically attaching event listenters. if you are using the
        /// Unity Editor, you should prefer <see cref="onClick"/>.
        /// </summary>
        public event EventHandler Click;

        public event EventHandler LongPress;

        /// <summary>
        /// Trigger the onClick/Click events, if they are valid.
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == clickButton && !IsDisabled)
            {
                GetPosition(eventData);
                OnClick();
            }
        }

        /// <summary>
        /// Trigger the onClick/click events, if they are valid;
        /// </summary>
        public virtual void OnClick()
        {
            onClick?.Invoke();
            Click?.Invoke(this, EventArgs.Empty);
        }

        public void ActivateClick()
        {
            if (!IsDisabled)
            {
                OnClick();
            }
        }

        public void OnLongPress(PointerEventData evt)
        {
            if (evt.button == clickButton && !IsDisabled)
            {
                OnLongPress();
            }
        }

        public virtual void OnLongPress()
        {
            onLongPress?.Invoke();
            LongPress?.Invoke(this, EventArgs.Empty);
        }

        public void OnLongPressUpdate(PointerEventData evt)
        {
            if (evt.button == clickButton && !IsDisabled)
            {
            }
        }
    }
}
