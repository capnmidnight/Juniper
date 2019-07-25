using System;
using System.Runtime.InteropServices;

using Juniper.XR;

using UnityEngine.Events;

#if UNITY_MODULES_XR

#endif

namespace Juniper.Display
{
    [Serializable]
    [ComVisible(false)]
    public class DisplayTypeEvent : UnityEvent<DisplayTypes>
    {
    }
}