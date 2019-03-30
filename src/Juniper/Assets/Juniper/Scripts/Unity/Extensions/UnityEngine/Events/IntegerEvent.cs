using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// An event that passes an integer value as an argument.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class IntegerEvent : UnityEvent<int>
    {
    }
}