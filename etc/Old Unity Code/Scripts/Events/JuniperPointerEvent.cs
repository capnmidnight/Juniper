using System;

using Juniper.Input;

using UnityEngine.Events;

namespace Juniper.Events
{
    /// <summary>
    /// Events that receive a <see cref="JuniperPointerEventData"/>
    /// object as a callback parameter.
    /// </summary>
    [Serializable]
    public class JuniperPointerEvent : UnityEvent<JuniperPointerEventData>
    {
    }
}
