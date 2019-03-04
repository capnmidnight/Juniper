#if UNITY_MODULES_PHYSICS

using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// A component that can be used to trigger events after an object hits a collider. Useful for
    /// detecting when a user gets near things.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggeredEvent : MonoBehaviour
    {
        /// <summary>
        /// Event callback arguments
        /// </summary>
        [Serializable]
        [ComVisible(false)]
        public class TriggeredEventDelegate : UnityEvent<Collider>
        {
        }

        /// <summary>
        /// The event to occur when an object enters the trigger.
        /// </summary>
        public TriggeredEventDelegate onEnter;

        /// <summary>
        /// The event to occur when an object leaves the trigger.
        /// </summary>
        public TriggeredEventDelegate onExit;

        /// <summary>
        /// Trigger the onEnter event, if it's valid.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            onEnter?.Invoke(other);
        }

        /// <summary>
        /// Trigger the onExit event, if it's valid.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            onExit?.Invoke(other);
        }
    }
}

#endif