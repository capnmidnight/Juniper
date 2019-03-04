using System;

using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Input
{
    /// <summary>
    /// This component listens for a given keystroke, firing an event when it occurs. That event can
    /// be used to trigger any
    /// <code>
    /// public void Method();
    /// </code>
    /// through the Unity Editor.
    /// </summary>
    public class KeyboardShortcut : MonoBehaviour
    {
        /// <summary>
        /// The keystroke to listen for. The event will be triggered on-key-down.
        /// </summary>
        public KeyCode key;

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

        /// <summary>
        /// Check for the key-press and trigger the activation event.
        /// </summary>
        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(key))
            {
                onActivated?.Invoke();
                Activated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}