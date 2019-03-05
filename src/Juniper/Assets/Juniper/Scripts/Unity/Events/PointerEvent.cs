using System;

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Events
{
    [Serializable]
    public class PointerEvent : UnityEvent<PointerEventData>
    {
    }
}
