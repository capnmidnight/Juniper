#if UNITY_XR_OCULUS

using UnityEngine;

namespace Juniper.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public InputMode SeatedVRMode = InputMode.SeatedVR;

        public override bool HasFloorPosition
        {
            get
            {
                return OVRPlugin.GetTrackingOriginType() != OVRPlugin.TrackingOrigin.EyeLevel;
            }
        }

        public override InputMode DefaultInputMode
        {
            get
            {
#if UNITY_EDITOR
                return InputMode.Desktop;
#else
                if(HasFloorPosition)
                {
                    return InputMode.StandingVR;
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