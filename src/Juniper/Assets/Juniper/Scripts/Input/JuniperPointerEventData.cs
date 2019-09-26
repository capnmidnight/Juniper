using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_MODULES_UI


#endif


namespace Juniper.Input
{
    public class JuniperPointerEventData : PointerEventData
    {
        public KeyCode keyCode;

        public JuniperPointerEventData(EventSystem eventSystem)
            : base(eventSystem) { }
    }
}