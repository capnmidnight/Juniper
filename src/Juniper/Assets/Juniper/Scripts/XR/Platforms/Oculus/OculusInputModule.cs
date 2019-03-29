#if UNITY_XR_OCULUS

namespace Juniper.Unity.Input
{
    public abstract class OculusInputModule : AbstractUnifiedInputModule
    {
        public override bool Install(bool reset)
        {
            if (base.Install(reset))
            {
                if (!reset && mode == Mode.Auto)
                {
#if UNITY_XR_OCULUS_ANDROID
                    mode = Mode.SeatedVR;
#else
                    mode = Mode.StandingVR;
#endif
                }

                return true;
            }

            return false;
        }
    }
}
#endif