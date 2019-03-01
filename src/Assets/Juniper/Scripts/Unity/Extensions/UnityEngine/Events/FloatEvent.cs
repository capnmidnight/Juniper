using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// An event that passes a float value as an argument.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class FloatEvent : UnityEvent<float>
    {
    }
}
