using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Juniper.Events
{
    public interface ILongPressHandler : IEventSystemHandler
    {
        void OnLongPressUpdate(PointerEventData evt);
        void OnLongPress(PointerEventData evt);
    }

    public static class LongPressEvents
    {
        private static void ExecuteLongPressUpdate(ILongPressHandler handler, BaseEventData eventData) =>
            handler.OnLongPressUpdate(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));

        public static ExecuteEvents.EventFunction<ILongPressHandler> longPressUpdateHandler =>
            ExecuteLongPressUpdate;

        private static void ExecuteLongPress(ILongPressHandler handler, BaseEventData eventData) =>
            handler.OnLongPress(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));

        public static ExecuteEvents.EventFunction<ILongPressHandler> longPressHandler =>
            ExecuteLongPress;
    }
}
