using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// An event that passes a boolean value as an argument.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class BooleanEvent : UnityEvent<bool>
    {
    }
}
