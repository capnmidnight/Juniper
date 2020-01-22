using System;
using System.Runtime.InteropServices;

using Juniper.XR;

using UnityEngine.Events;

namespace Juniper.Display
{
    [Serializable]
    [ComVisible(false)]
    public class DisplayTypeEvent : UnityEvent<DisplayTypes>
    {
    }
}