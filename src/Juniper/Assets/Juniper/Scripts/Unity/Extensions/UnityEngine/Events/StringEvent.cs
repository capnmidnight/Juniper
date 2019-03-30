using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// An event that passes a string value as an argument.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class StringEvent : UnityEvent<string>
    {
    }
}