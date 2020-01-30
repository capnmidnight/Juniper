using UnityEngine.EventSystems;

namespace Juniper.Events
{
    public interface ILongPressHandler : IEventSystemHandler
    {
        void OnLongPressUpdate(PointerEventData evt);

        void OnLongPress(PointerEventData evt);
    }
}
