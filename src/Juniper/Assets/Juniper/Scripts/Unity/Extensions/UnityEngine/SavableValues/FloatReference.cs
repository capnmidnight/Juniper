using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// A reference to either a savable FloatVariable or a static float value set in the editor.
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    public class FloatReference : AbstractReference<float, FloatVariable>
    {
    }
}