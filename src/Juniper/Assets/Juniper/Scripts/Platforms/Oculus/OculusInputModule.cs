#if UNITY_XR_OCULUS

namespace Juniper.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override bool HasFloorPosition
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                return !OVRDeviceSelector.isTargetDeviceGearVrOrGo;
#endif
            }
        }
        public override InputMode DefaultInputMode
        {
            get
            {

#if UNITY_EDITOR
                return InputMode.Desktop;
#else
                if(OVRDeviceSelector.isTargetDeviceGearVrOrGo)
                {
                    return Mode.SeatedVR;
                }
                else
                {
                    return Mode.StandingVR;
                }
#endif
            }
        }
    }
}

#endif