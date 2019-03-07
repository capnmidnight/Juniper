using UnityEngine;

namespace Juniper.Unity.Input
{
    [DisallowMultipleComponent]
    public class UnifiedInputModule :
#if GOOGLEVR
        DaydreamInputModule
#elif OCULUS
        OculusInputModule
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRInputModule
#else
        AbstractUnifiedInputModule
#endif
    {
    }
}
