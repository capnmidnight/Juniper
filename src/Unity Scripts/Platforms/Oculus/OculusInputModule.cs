#if UNITY_XR_OCULUS

namespace Juniper.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public InputModes SeatedVRMode = InputModes.SeatedVR;

        public override bool HasFloorPosition
        {
            get
            {
                return OVRPlugin.GetTrackingOriginType() != OVRPlugin.TrackingOrigin.EyeLevel;
            }
        }

        public override InputModes DefaultInputMode
        {
            get
            {
#if UNITY_EDITOR
                return InputModes.Desktop;
#else
                if(HasFloorPosition)
                {
                    return InputModes.StandingVR;
                }
                else
                {
                    return SeatedVRMode;
                }
#endif
            }
        }
    }
}

#endif