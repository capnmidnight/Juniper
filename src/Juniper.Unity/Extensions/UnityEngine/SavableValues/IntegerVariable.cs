using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// An integer value that can be saved to the Assets folder and used across projects
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    [CreateAssetMenu(fileName = "IntVar", menuName = "Variables/Integer")]
    public class IntegerVariable : AbstractVariable<int>
    {
    }
}
