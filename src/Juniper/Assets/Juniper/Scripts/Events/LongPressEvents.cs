using UnityEngine.EventSystems;

namespace Juniper.Events
{
    /// <summary>
    /// <see cref="ExecuteEvents.ExecuteHierarchy{T}(UnityEngine.GameObject, BaseEventData, ExecuteEvents.EventFunction{T})"/>
    /// requires parameters that are callbacks to execute
    /// event handlers on objects that it finds within
    /// the UI stack. This class holds those callbacks
    /// for the LongPress event.
    /// </summary>
    public static class LongPressEvents
    {
        /// <summary>
        /// Used by <see cref="ExecuteEvents.ExecuteHierarchy{T}(UnityEngine.GameObject, BaseEventData, ExecuteEvents.EventFunction{T})"/>
        /// to execute the <see cref="ILongPressHandler.OnLongPressUpdate(PointerEventData)"/>
        /// event handler for whatever objects it finds in the
        /// UI stack.
        /// </summary>
        /// <param name="handler">The object in the UI stack that will receive the event</param>
        /// <param name="eventData">The event data to pass to the event handler</param>
        private static void ExecuteLongPressUpdate(ILongPressHandler handler, BaseEventData eventData)
        {
            handler.OnLongPressUpdate(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        /// <summary>
        /// Used by <see cref="ExecuteEvents.ExecuteHierarchy{T}(UnityEngine.GameObject, BaseEventData, ExecuteEvents.EventFunction{T})"/>
        /// to execute the <see cref="ILongPressHandler.OnLongPressUpdate(PointerEventData)"/>
        /// event handler for whatever objects it finds in the
        /// UI stack.
        /// </summary>
        /// <param name="handler">The object in the UI stack that will receive the event</param>
        /// <param name="eventData">The event data to pass to the event handler</param>
        public static ExecuteEvents.EventFunction<ILongPressHandler> longPressUpdateHandler
        {
            get
            {
                return ExecuteLongPressUpdate;
            }
        }

        /// <summary>
        /// Used by <see cref="ExecuteEvents.ExecuteHierarchy{T}(UnityEngine.GameObject, BaseEventData, ExecuteEvents.EventFunction{T})"/>
        /// to execute the <see cref="ILongPressHandler.OnLongPress(PointerEventData)"/>
        /// event handler for whatever objects it finds in the
        /// UI stack.
        /// </summary>
        /// <param name="handler">The object in the UI stack that will receive the event</param>
        /// <param name="eventData">The event data to pass to the event handler</param>
        private static void ExecuteLongPress(ILongPressHandler handler, BaseEventData eventData)
        {
            handler.OnLongPress(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        /// <summary>
        /// Used by <see cref="ExecuteEvents.ExecuteHierarchy{T}(UnityEngine.GameObject, BaseEventData, ExecuteEvents.EventFunction{T})"/>
        /// to execute the <see cref="ILongPressHandler.OnLongPress(PointerEventData)"/>
        /// event handler for whatever objects it finds in the
        /// UI stack.
        /// </summary>
        /// <param name="handler">The object in the UI stack that will receive the event</param>
        /// <param name="eventData">The event data to pass to the event handler</param>
        public static ExecuteEvents.EventFunction<ILongPressHandler> longPressHandler
        {
            get
            {
                return ExecuteLongPress;
            }
        }
    }
}
