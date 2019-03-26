using UnityEngine;

namespace Juniper.Unity.Input
{
    [DisallowMultipleComponent]
    public class UnifiedInputModule :
#if GOOGLEVR
        DaydreamInputModule
#elif UNITY_XR_OCULUS
        OculusInputModule
#elif UNITY_XR_WINDOWSMR_METRO
        WindowsMRInputModule
#elif UNITY_XR_MAGICLEAP
        MagicLeapInputModule
#elif UNITY_ANDROID || UNITY_IOS
        AbstractMobileInputModule
#else
        AbstractUnifiedInputModule
#endif
    {
    }
}
