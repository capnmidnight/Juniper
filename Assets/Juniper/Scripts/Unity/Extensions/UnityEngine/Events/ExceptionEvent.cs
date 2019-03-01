using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// An event that passes a Transform value as an argument.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class ExceptionEvent : UnityEvent<Exception>
    {
    }
}
