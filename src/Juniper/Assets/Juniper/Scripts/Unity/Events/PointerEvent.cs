using System;

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Events
{
    [Serializable]
    public class PointerEvent : UnityEvent<PointerEventData>
    {
    }
}
