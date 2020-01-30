using UnityEngine.EventSystems;

namespace Juniper.Events
{

    public static class LongPressEvents
    {
        private static void ExecuteLongPressUpdate(ILongPressHandler handler, BaseEventData eventData)
        {
            handler.OnLongPressUpdate(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        public static ExecuteEvents.EventFunction<ILongPressHandler> longPressUpdateHandler
        {
            get
            {
                return ExecuteLongPressUpdate;
            }
        }

        private static void ExecuteLongPress(ILongPressHandler handler, BaseEventData eventData)
        {
            handler.OnLongPress(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        public static ExecuteEvents.EventFunction<ILongPressHandler> longPressHandler
        {
            get
            {
                return ExecuteLongPress;
            }
        }
    }
}
