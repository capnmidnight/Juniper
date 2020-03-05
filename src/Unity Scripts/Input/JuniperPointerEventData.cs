using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Input
{
    public class JuniperPointerEventData : PointerEventData
    {
        public KeyCode keyCode;

        public JuniperPointerEventData(EventSystem eventSystem)
            : base(eventSystem) { }
    }
}