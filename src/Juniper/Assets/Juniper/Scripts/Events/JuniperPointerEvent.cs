using System;

using Juniper.Input;

using UnityEngine.Events;

namespace Juniper.Events
{
    [Serializable]
    public class JuniperPointerEvent : UnityEvent<JuniperPointerEventData>
    {
    }
}
