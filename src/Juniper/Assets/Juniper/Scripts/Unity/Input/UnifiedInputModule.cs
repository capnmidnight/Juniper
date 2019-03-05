using UnityEngine;

namespace Juniper.Unity.Input
{
    [DisallowMultipleComponent]
    public class UnifiedInputModule :
#if GOOGLEVR
        DaydreamInputModule
#elif OCULUS
        OculusInputModule
#elif UNITY_WSA && (WINDOWSMR || HOLOLENS)
        WindowsMRInputModule
#else
        AbstractUnifiedInputModule
#endif
    {
    }
}
