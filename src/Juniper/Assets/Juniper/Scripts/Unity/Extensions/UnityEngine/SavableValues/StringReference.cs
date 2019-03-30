using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// A reference to either a savable StringVariable or a static string value set in the editor.
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    public class StringReference : AbstractReference<string, StringVariable>
    {
    }
}