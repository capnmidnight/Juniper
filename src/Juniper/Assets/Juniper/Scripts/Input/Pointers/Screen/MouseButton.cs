#if UNITY_EDITOR || UNITY_WSA || UNITY_STANDALONE || UNITY_WEBGL
#define HAS_MOUSE
#endif

using UnityEngine;

namespace Juniper.Input.Pointers.Screen
{
    public enum MouseButton
    {
        None = KeyCode.None,
        Mouse0 = KeyCode.Mouse0,
        Mouse1 = KeyCode.Mouse1,
        Mouse2 = KeyCode.Mouse2,
        Mouse3 = KeyCode.Mouse3,
        Mouse4 = KeyCode.Mouse4,
        Mouse5 = KeyCode.Mouse5,
        Mouse6 = KeyCode.Mouse6
    }
}
