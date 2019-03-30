using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// A string value that can be saved to the Assets folder and used across projects
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    [CreateAssetMenu(fileName = "StringVar", menuName = "Variables/String")]
    public class StringVariable : AbstractVariable<string>
    {
        public static implicit operator bool(StringVariable value)
        {
            return value != null && !string.IsNullOrEmpty(value.Value);
        }
    }
}