using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Events
{
    /// <summary>
    /// An event that passes an Vector3 value as an argument.
    /// </summary>
    [Serializable]
    [ComVisible(false)]
    public class Vector3Event : UnityEvent<Vector3>
    {
    }
}
