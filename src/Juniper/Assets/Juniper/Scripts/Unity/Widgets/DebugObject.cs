using UnityEngine;

namespace Juniper.Unity.Widgets
{
    /// <summary>
    /// GameObjects with this component will be hidden in the live application view, only appearing
    /// in the Unity Editor.
    /// </summary>
    public class DebugObject : MonoBehaviour
    {
#if !UNITY_EDITOR
        /// <summary>
        /// Deactivates the object when not running in the editor.
        /// </summary>
        void Awake()
        {
            this.Deactivate();
        }
#endif
    }
}
