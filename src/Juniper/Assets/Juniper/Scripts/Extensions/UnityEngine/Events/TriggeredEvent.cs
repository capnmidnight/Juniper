#if UNITY_MODULES_PHYSICS

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
        public void OnTriggerEnter(Collider other)
        {
            onEnter?.Invoke(other);
        }

        /// <summary>
        /// Trigger the onExit event, if it's valid.
        /// </summary>
        /// <param name="other"></param>
        public void OnTriggerExit(Collider other)
        {
            onExit?.Invoke(other);
        }
    }
}

#endif