using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    /// <summary>
    /// A float value that can be saved to the Assets folder and used across projects
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    [CreateAssetMenu(fileName = "FloatVar", menuName = "Variables/Float")]
    public class FloatVariable : AbstractVariable<float>
    {
    }
}
