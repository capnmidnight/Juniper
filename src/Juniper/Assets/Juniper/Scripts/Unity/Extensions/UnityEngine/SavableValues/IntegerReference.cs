using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// A reference to either a savable IntegerVariable or a static int value set in the editor.
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    public class IntegerReference : AbstractReference<int, IntegerVariable>
    {
    }
}
