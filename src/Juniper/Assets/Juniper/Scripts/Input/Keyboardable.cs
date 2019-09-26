
using UnityEngine;

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
    public class Keyboardable : AbstractShortcutable, IKeyboardTriggered
    {
        /// <summary>
        /// The keystroke to listen for. The event will be triggered on-key-down.
        /// </summary>
        public KeyCode key;

        public KeyCode KeyCode
        {
            get
            {
                return key;
            }
        }

        protected override void OnDisable()
        {
            if(input != null)
            {
                input.RemoveKeyboardShortcut(this);
            }
        }

        protected override void OnEnable()
        {
            if(input != null)
            {
                input.AddKeyboardShortcut(this);
            }
        }
    }
}
