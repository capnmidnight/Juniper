using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// Event callback arguments
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class TriggeredEventDelegate : UnityEvent<Collider>
    {
    }
}