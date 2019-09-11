#if UNITY_XR_OCULUS

namespace Juniper.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override void Install(bool reset)
        {
            base.Install(reset);

            if (!reset && mode == InputMode.Auto)
            {
#if UNITY_EDITOR
                mode = InputMode.Desktop;
#else
                if(OVRDeviceSelector.isTargetDeviceGearVrOrGo)
                {
                    mode = Mode.SeatedVR;
                }
                else
                {
                    mode = Mode.StandingVR;
                }
#endif
            }
        }

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
    }
}

#endif