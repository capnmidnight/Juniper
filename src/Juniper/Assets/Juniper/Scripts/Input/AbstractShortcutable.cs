using System;
using Juniper.Widgets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Input
{
    public abstract class AbstractShortcutable : MonoBehaviour
    {
        /// <summary>
        /// A default function for when the Keywordable is applied to something that
        /// does not have a parent control.
        /// </summary>
        /// <returns></returns>
        private static bool AlwaysEnabled()
        {
            return true;
        }

        /// <summary>
        /// The event that fires when the shortcut key is activated. You should only prefer to use
        /// this version of the event over <see cref="Activated"/> in the Unity Editor, where
        /// standard .NET events are inexplicably unavailable.
        /// </summary>
        public UnityEvent onActivated = new UnityEvent();

        /// <summary>
        /// The event that fires when the shortcut key is activated. You should prefer to use this
        /// event over <see cref="onActivated"/> whenever possible, such as wiring up events programmatically.
        /// </summary>
        public event EventHandler Activated;


        protected UnifiedInputModule input;

        public bool interactable = true;
        private Func<bool> isParentEnabled;
        private bool wasInteractable;

        public virtual bool IsInteractable()
        {
            return isActiveAndEnabled
                && interactable
                && input != null
                && isParentEnabled();
        }

        protected virtual void Awake()
        {
            Find.Any(out input);

            var parentControl = GetComponent<IPointerClickHandler>();
            if (parentControl is UnityEngine.UI.Selectable selectable)
            {
                isParentEnabled = selectable.IsInteractable;
            }
            else if (parentControl is AbstractTouchable touchable)
            {
                isParentEnabled = touchable.IsInteractable;
            }
            else
            {
                isParentEnabled = AlwaysEnabled;
            }
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();

        protected virtual void Update()
        {
            var isInteractable = IsInteractable();
            if (isInteractable != wasInteractable)
            {
                wasInteractable = isInteractable;
                if (isInteractable)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        public void ActivateEvent()
        {
            if (IsInteractable())
            {
                onActivated?.Invoke();
                Activated?.Invoke(this, EventArgs.Empty);
                input.ClickButtons(GetComponents<IPointerClickHandler>());
            }
        }
    }
}
